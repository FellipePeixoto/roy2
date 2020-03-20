using UnityEngine;

[CreateAssetMenu(fileName = "IntSO", menuName = "Scriptable/Vars/IntSO")]
public class IntSO : ScriptableObject
{
    int ivalue;
    public OnValueChangeDelegate OnValueChange;
    public delegate void OnValueChangeDelegate(int newValue);

    public int Value
    {
        get
        {
            return ivalue;
        }
        set
        {
            ivalue = value;
            OnValueChange?.Invoke(ivalue);
        }
    }
}
