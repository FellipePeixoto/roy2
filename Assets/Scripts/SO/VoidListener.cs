using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidListener : MonoBehaviour
{
    [SerializeField] VoidSO voidSO;
    [SerializeField] UnityEvent onRaised;

    private void Awake()
    {
        voidSO.onRaise = () =>
        {
            onRaised.Invoke();
        };
    }
}
