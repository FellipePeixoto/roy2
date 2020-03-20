using UnityEngine;

[CreateAssetMenu(fileName = "Float Variable Value", menuName = "Global Values System/Variable Values/Float")]
public class FloatVarSO : VariableValueSO<float>
{
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("Float numbers wich the difference is mininal are equals?")] bool _nearValues;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("Restrict value to the range?")] bool _inRange;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The minimum value of range")] float _min;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The maximum value of range")] float _max;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura

    public new float Value
    {
        get
        {
            return myValue;
        }

        set
        {
            onSetValue?.Invoke(value);

            //TODO: Min e Max com Approximately
            if (_inRange)
                value = Mathf.Clamp(value, _min, _max);

            if (myValue != value)
            {
                onValueChanged?.Invoke(value);
                myValue = value;
                return;
            }

            if (_nearValues && !Mathf.Approximately(myValue, value))
            {
                onValueChanged?.Invoke(value);
                myValue = value;
            }
        }
    }

    public void Add(float value)
    {
        Value += value;
    }
}
