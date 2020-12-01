using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionController : MonoBehaviour
{
    [SerializeField] PhysicMaterial _hardFric;

    float _defaultStaticFric;
    float _defaultDynamicFric;

    private void Awake()
    {
        _defaultDynamicFric = _hardFric.dynamicFriction;
        _defaultStaticFric = _hardFric.staticFriction;
    }

    public void OnZeroFriction()
    {
        _hardFric.dynamicFriction = 0;
        _hardFric.staticFriction = 0;
    }

    public void OnRestoreFriction()
    {
        _hardFric.dynamicFriction = _defaultDynamicFric;
        _hardFric.staticFriction = _defaultStaticFric;
    }

    private void OnDisable()
    {
        _hardFric.dynamicFriction = _defaultDynamicFric;
        _hardFric.staticFriction = _defaultStaticFric;
    }
}
