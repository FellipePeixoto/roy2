using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharChooserController : MonoBehaviour
{
    [SerializeField] Characters _whatCharacter;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] int player;
    [SerializeField] UnityEvent onChooseSucessful;
    [SerializeField] UnityEvent onUnchooseSucessful;

    Button _button;

    public void TryToChoose(BaseEventData eventData)
    {
        if (PairingManager.Instance.PairDeviceToPlayer(player, _whatCharacter, _playerInput.devices[0]))
        {
            onChooseSucessful.Invoke();
        }
    }

    public void TryUnchoose()
    {
        if (PairingManager.Instance.UnpairDeviceFromPlayer(player))
            onUnchooseSucessful.Invoke();
    }
}
