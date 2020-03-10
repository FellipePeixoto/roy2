using UnityEngine;

[System.Serializable]
public class Equals
{
    public int value;
    public UnityEventInt ifEqualsDo;
}

public class IntListenerSO : MonoBehaviour
{
    [SerializeField] IntSO intSO;
    [SerializeField] UnityEventInt onChangeValue;
    [SerializeField] UnityEventString onChangeValueString;
    [Header("Custom Conditions")]
    [SerializeField] Equals[] equals;

    private void Awake()
    {
        if (!intSO)
            return;

        intSO.OnValueChange = (int value) =>
        {
            foreach (Equals e in equals)
            {
                if (e.value == value)
                {
                    e.ifEqualsDo.Invoke(value);
                }
            }

            onChangeValueString.Invoke(value.ToString());
            onChangeValue.Invoke(value);
        };
    }
}