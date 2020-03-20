using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(InputSystemUIInputModule))]
public class NavLockController : MonoBehaviour
{
    InputSystemUIInputModule _inputUIModule;

    InputActionReference _inputReferenceToNav;

    private void Awake()
    {
        _inputUIModule = GetComponent<InputSystemUIInputModule>();
        _inputReferenceToNav = _inputUIModule.move;
    }

    public void LockNavigate()
    {
        _inputUIModule.move = null;
    }

    public void UnlockNavigate()
    {
        _inputUIModule.move = _inputReferenceToNav;
    }
}
