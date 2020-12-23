using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KlunkInteractController : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;

    [Header("Pick/Drop trash configs")]
    [SerializeField] float _timeToPickOrDrop = .75f;
    [SerializeField] float _pickRadius = 2;
    [SerializeField] LayerMask _collectMask = 1 << 10;
    [SerializeField] Color _debbugCastColor = Color.green;
    [SerializeField] Vector3 _dropPositionOffset = (Vector3.right * .2f) + Vector3.one;
    [SerializeField] LayerMask _dropMask = (1 << 0) + (1 << 8);
    [SerializeField] Color _debbugDropTrashColor = Color.cyan;
    [Space]

    [SerializeField] GameObject _batteryPrefab;

    public Trash CurrentTrash { get; private set; }

    Vector3 _dir = Vector3.right;
    Vector3 _trashSize = Vector3.one;
    Color _currentDebbugColorTrash = Color.clear;
    Klunk _klunk;

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
        _batteryPrefab = Resources.Load<GameObject>("Prefabs/Set_Battery");
    }

    private void Awake()
    {
        _klunk = GetComponent<Klunk>();
        _klunk.ActionInteract.performed += _actionInteract_performed;
        _klunk.ActionSupport.performed += _actionSupport_performed;
    }

    private void _actionSupport_performed(InputAction.CallbackContext obj)
    {
        if (CurrentTrash != null)
        {
            CurrentTrash.DestroyTrash();
            CurrentTrash = null;
            Instantiate(_batteryPrefab,
                transform.position + transform.up * (transform.localScale.y),
                Quaternion.identity);
        }
    }

    private void _actionInteract_performed(InputAction.CallbackContext obj)
    {
        if (_klunk.ActionsLocked)
            return;

        if (CurrentTrash != null && CanDropTrash(_trashSize))
        {
            _currentDebbugColorTrash = Color.clear;
            Vector3 offset = new Vector3(transform.right.x * _dropPositionOffset.x, transform.right.y * _dropPositionOffset.y);
            CurrentTrash.DropTrash(_mainCollider.bounds.center + offset);
            CurrentTrash = null;
            _klunk.LockActions(true);
            StartCoroutine(TimeToPickOrDropTrash());
            return;
        }

        Collider[] cols = Scan();

        if (CurrentTrash == null && cols.Length > 0)
        {
            Trash trash = PickNearest(cols).gameObject.GetComponentInParent<Trash>();
            _trashSize = trash.TrashCollider().bounds.size;
            trash.HideTrash();
            CurrentTrash = trash;
            _currentDebbugColorTrash = _debbugDropTrashColor;
            _klunk.LockActions(true);
            StartCoroutine(TimeToPickOrDropTrash());
        }
    }

    IEnumerator TimeToPickOrDropTrash()
    {
        yield return new WaitForSeconds(_timeToPickOrDrop);
        _klunk.LockActions(false);
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

    private void OnDestroy()
    {
        _klunk.ActionInteract.performed -= _actionInteract_performed;
        _klunk.ActionSupport.performed -= _actionSupport_performed;
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
