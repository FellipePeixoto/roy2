using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatListenerSO : MonoBehaviour
{
    [SerializeField] FloatVarSO floatSO;
    [SerializeField] UnityEventFloat onChangeValue;

    private void OnEnable()
    {
        floatSO.onValueChanged += ctx => onChangeValue.Invoke(ctx);
    }
}
