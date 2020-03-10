using UnityEngine;

[CreateAssetMenu(fileName = "FloatSO",menuName = "Scriptable/Vars/FloatSO")]
public class FloatSO : ScriptableObject
{
    [SerializeField] float min;
    [SerializeField] float max;

    float fvalue;
    public OnValueChangeDelegate OnValueChange;
    public delegate void OnValueChangeDelegate(float newValue);


    public float Value
    {
        get
        {
            return fvalue;
        }
        set
        {
            float pastValue = fvalue;

            if (min != max)
            {
                fvalue = Mathf.Clamp(value, min, max);
            }

            fvalue = value;

            if (pastValue != fvalue)
                OnValueChange?.Invoke(fvalue);
        }
    }
}
