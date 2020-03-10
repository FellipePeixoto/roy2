using UnityEngine;

public class BoolListener : MonoBehaviour
{
    [SerializeField] BoolSO boolSO;
    [SerializeField] UnityEventBool onChangeValue;

    private void Awake()
    {
        boolSO.OnValueChange = (bool value) =>
        {
            onChangeValue.Invoke(value);
        };
    }
}