using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatListenerSO : MonoBehaviour
{
    [SerializeField] FloatVarSO floatSO;
    [SerializeField] UnityEventFloat onChangeValue;

    private void Awake()
    {
        floatSO.onValueChanged = (float value) =>
        {
            onChangeValue.Invoke(value);
        };

        floatSO.onSetValue = (float value) =>
        {
            onChangeValue.Invoke(value);
        };
    }
}
