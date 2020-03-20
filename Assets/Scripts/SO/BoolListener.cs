using UnityEngine;

public class BoolListener : MonoBehaviour
{
    [SerializeField] BoolVarSO boolSO;
    [SerializeField] UnityEventBool onChangeValue;

    private void Awake()
    {
        boolSO.onSetValue = (bool value) =>
        {
            onChangeValue.Invoke(value);
        };

        boolSO.onValueChanged = (bool value) =>
        {
            onChangeValue.Invoke(value);
        };
    }
}