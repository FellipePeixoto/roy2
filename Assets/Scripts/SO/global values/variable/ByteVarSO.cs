using UnityEngine;

[CreateAssetMenu(fileName = "Byte Variable Value", menuName = "Global Values System/Variable Values/Byte")]
public class ByteVarSO : VariableValueSO<byte>
{
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The minimum value of this")] byte min;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura
#pragma warning disable IDE0044 // Adicionar modificador somente leitura
    [SerializeField] [Tooltip("The maximum value of this")] byte max;
#pragma warning restore IDE0044 // Adicionar modificador somente leitura

    public new byte Value
    {
        get
        {
            return myValue;
        }

        set
        {
            onSetValue?.Invoke(value);

            //TODO: Min e Max com Approximately
            value = (byte) Mathf.Clamp(value, min, max);

            if (myValue != value)
            {
                onValueChanged?.Invoke(value);
                myValue = value;
            }
        }
    }
}