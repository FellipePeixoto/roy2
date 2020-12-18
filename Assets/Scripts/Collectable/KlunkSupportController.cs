using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KlunkSupportController: MonoBehaviour
{
    [SerializeField] Collider _mainCollider;

    [Header("Is grounded configs")]
    [SerializeField] float _radiusOffset = .99f;
    [SerializeField] float _maxDistanceHit = 2;
    [SerializeField] LayerMask _castLayerMask;
    [SerializeField] Color _debbugCastColor = Color.green;
    RaycastHit _hitInfo;
    [Space]

    [SerializeField] GameObject _batteryPrefab;

    public bool TrashAhead { get; private set; }
    public Trash CurrentTrash { get; private set; }

    Trash _availableToCollect;

    InputAction _actionMove;
    InputAction _actionInteract;
    InputAction _actionSupport;
    Vector3 _dir = Vector3.right;

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
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
            Instantiate(_batteryPrefab, 
                transform.position + transform.up * (transform.localScale.y), 
                Quaternion.identity);
        }
    }

    private void _actionInteract_performed(InputAction.CallbackContext obj)
    {
        if (CurrentTrash == null && TrashAhead)
        {
            _availableToCollect.HideTrash();
            CurrentTrash = _availableToCollect;
            _availableToCollect = null;
        }
        else if (_actionInteract.triggered && CurrentTrash != null)
        {
            CurrentTrash.transform.position = transform.position + transform.right * transform.localScale.x;
            CurrentTrash.ShowTrash();
            CurrentTrash = null;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_actionMove.ReadValue<Vector2>().Equals(Vector2.zero))
        {
            _dir = _actionMove.ReadValue<Vector2>();
        }
        Scan(_dir);
    }

    bool Scan(Vector3 dir)
    {
        TrashAhead = false;
        _availableToCollect = null;
        bool hit = Physics.SphereCast(_mainCollider.bounds.center, 
            _mainCollider.bounds.extents.x * _radiusOffset, _dir, 
            out _hitInfo, 
            _maxDistanceHit, 
            _castLayerMask);
        if (hit && _hitInfo.collider.GetComponent<Trash>())
        {
            TrashAhead = true;
            _availableToCollect = _hitInfo.collider.GetComponent<Trash>();
            return TrashAhead;
        }
        return TrashAhead;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _debbugCastColor;

        //Check if there has been a hit yet
        if (Scan(_dir))
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(_mainCollider.bounds.center, _dir * _hitInfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireSphere(_mainCollider.bounds.center + _dir * _hitInfo.distance, _mainCollider.bounds.extents.x * _radiusOffset);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(_mainCollider.bounds.center, _dir * _maxDistanceHit);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireSphere(_mainCollider.bounds.center + _dir * _maxDistanceHit, _mainCollider.bounds.extents.x * _radiusOffset);
        }
    }
}
