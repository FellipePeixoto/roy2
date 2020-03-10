using UnityEngine;

[CreateAssetMenu(fileName = "Short int Variable Value", menuName = "Global Values System/Variable Values/Short int")]
public class ShortVarSO : VariableValueSO<short>
{
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("Restrict value to the range?")] bool inRange;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The minimum value of range")] short min;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The maximum value of range")] short max;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura

    public new short Value
    {
        get
        {
            return myValue;
        }

        set
        {
            onSetValue?.Invoke(value);

            if (inRange && value < min)
            {
                value = min;
            }
            else if (inRange && value > max)
            {
                value = max;
            }

            if (myValue != value)
            {
                onValueChanged?.Invoke(value);
                myValue = value;
            }
        }
    }
}
