using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DisableColliderHandler(Collider col);

public class DisabledCollider : MonoBehaviour
{
    public event DisableColliderHandler OnDisableCollider;

    Collider _collider;
    bool _triggered;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (!_triggered && _collider.enabled == false)
        {
            OnDisableCollider?.Invoke(_collider);
            _triggered = true;
        }
    }
}
