using UnityEngine;

[CreateAssetMenu(fileName = "BoolSO", menuName = "Scriptable/Vars/BoolSO")]
public class BoolSO : ScriptableObject
{
    bool bvalue;
    public OnValueChangeDelegate OnValueChange;
    public delegate void OnValueChangeDelegate(bool newValue);

    public bool Value
    {
        get
        {
            return bvalue;
        }
        set
        {
            bvalue = value;
            OnValueChange?.Invoke(bvalue);
        }
    }
}