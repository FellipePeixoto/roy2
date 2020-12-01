using UnityEngine;
using UnityEngine.InputSystem;

public enum Characters { None, Roy, Klunk}

public class PairingManager : MonoBehaviour
{
    static PairingManager _instance;

    public static PairingManager Instance { get => _instance; }

    [SerializeField] IntVarSO _pairedPlayers;

    PlayerControlInfo _player1;
    PlayerControlInfo _player2;

    InputDevice[] inputDevices;

    struct PlayerControlInfo
    {
        public Characters IAm;
        public InputDevice myInputDevice;
    }

    private void Awake()
    {
        if (_instance == null && _instance != this)
        {
            _instance = this;
            _player1 = new PlayerControlInfo() { IAm = Characters.None, myInputDevice = null};
            _player2 = new PlayerControlInfo() { IAm = Characters.None, myInputDevice = null};
            DontDestroyOnLoad(_instance);


            //TODO: remover essa gabiarra
#if UNITY_EDITOR
            inputDevices = InputSystem.devices.ToArray();

            if (inputDevices.Length == 2)
            {
                _player1.IAm = Characters.Roy;
                _player2.IAm = Characters.Klunk;
                _player1.myInputDevice = inputDevices[0];
                _player2.myInputDevice = inputDevices[1];
            }
#endif

            return;
        }

        Destroy(_instance);
    }

    public bool PairDeviceToPlayer(int player, Characters iAm, InputDevice device)
    {
        if (_player1.IAm == Characters.None && player == 1 && _player2.IAm != iAm)
        {
            _player1.IAm = iAm;
            _player1.myInputDevice = device;
            _pairedPlayers.Value++;
            return true;
        }
        
        if (_player2.IAm == Characters.None && player == 2 && _player1.IAm != iAm)
        {
            _player2.IAm = iAm;
            _player2.myInputDevice = device;
            _pairedPlayers.Value++;
            return true;
        }

        return false;
    }

    public bool UnpairDeviceFromPlayer(int player)
    {
        if (_player1.IAm != Characters.None && player == 1)
        {
            _player1.IAm = Characters.None;
            _player1.myInputDevice = null;
            _pairedPlayers.Value--;
            return true;
        }

        if (_player2.IAm != Characters.None && player == 2)
        {
            _player2.IAm = Characters.None;
            _player2.myInputDevice = null;
            _pairedPlayers.Value--;
            return true;
        }

        return false;
    }

    public InputDevice[] TakeControlOf(Characters who)
    {
        switch (who)
        {
            case Characters.Klunk:

                if (_player1.IAm == Characters.Klunk)
                    return new InputDevice[] { _player1.myInputDevice };

                if (_player2.IAm == Characters.Klunk)
                    return new InputDevice[] { _player2.myInputDevice };

                break;

            case Characters.Roy:

                if (_player1.IAm == Characters.Roy)
                    return new InputDevice[] { _player1.myInputDevice };

                if (_player2.IAm == Characters.Roy)
                    return new InputDevice[] { _player2.myInputDevice };

                break;

            default:

                return InputSystem.devices.ToArray();
        }

        return InputSystem.devices.ToArray();
    }
}
