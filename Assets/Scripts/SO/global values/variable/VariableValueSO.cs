using System.Collections.Generic;

public class VariableValueSO<T> : GlobalValueSO<T>
{
    public delegate void valueHandler(T newValue);
    public valueHandler onSetValue;
    public valueHandler onValueChanged;

    public new T Value
    {
        get
        {
            return myValue;
        }

        set
        {
            onSetValue?.Invoke(value);
            if (!EqualityComparer<T>.Default.Equals(myValue, value))
            {
                onValueChanged?.Invoke(value);
                myValue = value;
            }
        }
    }
}