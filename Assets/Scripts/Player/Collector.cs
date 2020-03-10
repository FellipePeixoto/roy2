using UnityEngine;
using UnityEngine.Events;

public class Collector : MonoBehaviour
{
    [SerializeField] UnityEvent _onPickBattery;
    [SerializeField] UnityEvent _onPickBottle;
    [SerializeField] UnityEvent _onPickCard;
    [SerializeField] UnityEvent _onPickCereal;
    [SerializeField] UnityEvent _onPickTrash;

    public void SetTypeCollected(Collectables type)
    {
        switch (type)
        {
            case Collectables.Battery:
                _onPickBattery.Invoke();
                break;

            case Collectables.Bottle:
                _onPickBottle.Invoke();
                break;

            case Collectables.Card:
                _onPickCard.Invoke();
                break;

            case Collectables.Cereal:
                _onPickCereal.Invoke();
                break;

            case Collectables.Thrash:
                _onPickTrash.Invoke();
                break;

#if UNITY_EDITOR
            default:
                Debug.LogError("Han?");
                break;
#endif
        }
    }
}
