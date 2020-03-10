using UnityEngine;

[CreateAssetMenu(fileName = "Float Variable Value", menuName = "Global Values System/Variable Values/Float")]
public class FloatVarSO : VariableValueSO<float>
{
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("Float numbers wich the difference is mininal are equals?")] bool nearValues;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("Restrict value to the range?")] bool inRange;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The minimum value of range")] float min;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The maximum value of range")] float max;
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
            if (inRange)
                value = Mathf.Clamp(value, min, max);

            if (myValue != value)
            {
                onValueChanged?.Invoke(value);
                myValue = value;
                return;
            }

            if (nearValues && !Mathf.Approximately(myValue, value))
            {
                onValueChanged?.Invoke(value);
                myValue = value;
            }
        }
    }
}
