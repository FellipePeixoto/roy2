using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Void SO", menuName = "Scriptable/Vars/VoidSO")]
public class VoidSO : ScriptableObject
{
    public OnRaiseDelegate onRaise;
    public delegate void OnRaiseDelegate();

    public void Raise()
    {
        onRaise?.Invoke();
    }
}
