using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoyInteractController : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;

    [Header("Pick/Drop trash configs")]
    [SerializeField] float _pickRadius = 2;
    [SerializeField] LayerMask _collectMask = 1 << 10;
    [SerializeField] Color _debbugCastColor = Color.green;
    [SerializeField] Vector3 _dropPositionOffset = (Vector3.right * .2f) + Vector3.one;
    [SerializeField] LayerMask _dropMask = (1 << 8);
    [SerializeField] Color _debbugDropTrashColor = Color.cyan;
    [SerializeField] Animator _animator;
    [Space]

    [SerializeField] GameObject _bottlePrefab;

    public Trash CurrentTrash { get; private set; }

    InputAction _actionMove;
    InputAction _actionInteract;
    InputAction _actionSupport;
    Vector3 _dir = Vector3.right;
    Vector3 _trashSize = Vector3.one;
    private Color _currentDebbugColorTrash = Color.clear;

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
        _bottlePrefab = Resources.Load<GameObject>("Prefabs/Set_Bottle");
    }

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        _actionMove = playerInput.actions["Move"];
        _actionInteract = playerInput.actions["Interaction"];
        _actionInteract.performed += _actionInteract_performed;
        _actionSupport = playerInput.actions["Support"];
        _actionSupport.performed += _actionSupport_performed;
    }

    private void _actionSupport_performed(InputAction.CallbackContext obj)
    {
        if (CurrentTrash != null)
        {
            CurrentTrash.DestroyTrash();
            CurrentTrash = null;
            Instantiate(_bottlePrefab,
                transform.position + transform.up * (transform.localScale.y),
                Quaternion.identity);
        }
    }

    private void _actionInteract_performed(InputAction.CallbackContext obj)
    {
        if (CurrentTrash != null && CanDropTrash(_trashSize))
        {
            _animator.Play("Roy_Armature_Descompactar  ");
            _currentDebbugColorTrash = Color.clear;
            Vector3 offset = new Vector3(transform.right.x * _dropPositionOffset.x, transform.right.y * _dropPositionOffset.y);
            CurrentTrash.DropTrash(_mainCollider.bounds.center + offset);
            CurrentTrash = null;
            return;
        }

        Collider[] cols = Scan();

        if (CurrentTrash == null && cols.Length > 0)
        {
            _animator.Play("Roy_Armature_coletar");
            Trash trash = PickNearest(cols).gameObject.GetComponentInParent<Trash>();
            _trashSize = trash.TrashCollider().bounds.size;
            trash.HideTrash();
            CurrentTrash = trash;
            _currentDebbugColorTrash = _debbugDropTrashColor;
        }
    }

    bool CanDropTrash(Vector3 trashSize)
    {
        Vector3 offset = new Vector3(transform.right.x * _dropPositionOffset.x, transform.right.y * _dropPositionOffset.y);
        return Physics.OverlapBox(_mainCollider.bounds.center + (offset),
            trashSize / 2, 
            Quaternion.identity, 
            _dropMask).Length == 0;
    }

    Collider[] Scan()
    {
        return Physics.OverlapSphere(_mainCollider.bounds.center, _pickRadius, _collectMask);
    }

    Collider PickNearest(Collider[] cols)
    {
        Collider nearest = cols[0];

        for (int i = 1; i < cols.Length; i++)
        {
            if (Vector3.Distance(transform.position, cols[i].transform.position) <
                Vector3.Distance(transform.position, cols[i - 1].transform.position))
            {
                nearest = cols[i];
            }
        }

        return nearest;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _debbugCastColor;
        Gizmos.DrawWireSphere(_mainCollider.bounds.center, _pickRadius);
        //Gizmos.color = _currentDebbugColorTrash;
        Gizmos.color = _currentDebbugColorTrash;
        Vector3 offset = new Vector3(transform.right.x * _dropPositionOffset.x, transform.right.y * _dropPositionOffset.y);
        Gizmos.DrawWireCube(_mainCollider.bounds.center + offset, _trashSize);
    }
}
