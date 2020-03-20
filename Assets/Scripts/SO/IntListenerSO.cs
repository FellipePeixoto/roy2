using UnityEngine;

[System.Serializable]
public class Equals
{
    public int value;
    public UnityEventInt ifEqualsDo;
}

public class IntListenerSO : MonoBehaviour
{
    [SerializeField] IntVarSO intSO;
    [SerializeField] UnityEventInt onChangeValue;

    private void Awake()
    {
        if (!intSO)
            return;

        intSO.onSetValue = (int value) =>
        {
            onChangeValue.Invoke(value);
        };

        intSO.onValueChanged = (int value) =>
        {
            onChangeValue.Invoke(value);
        };
    }
}