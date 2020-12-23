using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionController : MonoBehaviour
{
    [SerializeField] PhysicMaterial _softFric;
    [SerializeField] PhysicMaterial _hardFric;

    Collider[] _cols;

    float _defaultStaticFric;
    float _defaultDynamicFric;

    private void Awake()
    {
        _cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in _cols)
        {
            col.material = _hardFric;
        }
    }

    void SetFric(PhysicMaterial target)
    {
        foreach (Collider col in _cols)
        {
            col.material = target;
        }
    }

    public void SetZeroFriction()
    {
        SetFric(_softFric);
    }

    public void SetMaxFriction()
    {
        SetFric(_hardFric);
    }

    public void OnZeroFriction()
    {
        //_hardFric.dynamicFriction = 0;
        //_hardFric.staticFriction = 0;
    }

    public void OnRestoreFriction()
    {
        //_hardFric.dynamicFriction = _defaultDynamicFric;
        //_hardFric.staticFriction = _defaultStaticFric;
    }

    private void OnDisable()
    {
        SetFric(_hardFric);
    }
}
