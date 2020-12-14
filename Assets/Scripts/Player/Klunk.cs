using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum KlunkState
{
    None,
    Dashing,
    Sk8erBoi
}

public class Klunk : MonoBehaviour
{
    [SerializeField] int _fuel = 100;
    [Tooltip("In Seconds")]
    [SerializeField] int _fuelTimeDuration = 600;
    [Tooltip("investida com seu escudo energizado")]
    [SerializeField] int _dashFuel = 20;
    [SerializeField] float _dashDistance = 10;
    [SerializeField] float _dashSpeed = 35;

    [SerializeField] int _sk8erBoiFuelPerSecond = 10;
    [SerializeField] float _sk8Speed = 25;

    [SerializeField] float _speed = 15;
    [SerializeField] float _jumpHeight = 6;

    KlunkState _currentState;
    FrontAttack _frontAttack;
    CharacterController _characterController;

    InputAction _actionMove;
    InputAction _actionInteract;
    InputAction _actionJump;
    InputAction _actionMov2;
    InputAction _actionMov1;
    InputAction _actionSupport;
    InputAction _actionOpenMap;


    float _fuelPerSecond;
    float _remainingDashTime;
    float _fuelConsumed;    
    bool _actionMov2Pressed;
    bool _hitWallWhileDash;

    private void Awake()
    {
        _frontAttack = GetComponentInChildren<FrontAttack>();
        _frontAttack.gameObject.SetActive(false);
        _frontAttack.OnTriggerEnterEvent += _frontAttackChecker_OnTriggerEnterEvent;
        _fuelPerSecond = (float) _fuel / _fuelTimeDuration;
        _currentState = KlunkState.None;
        _characterController = GetComponent<CharacterController>();
        var playerInput = GetComponent<PlayerInput>();
        _actionMove = playerInput.actions["Move"];
        _actionJump = playerInput.actions["Jump"];
        _actionMov1 = playerInput.actions["Mov1"];
        _actionMov2 = playerInput.actions["Mov2"];
        _actionMov2.performed += _actionMov2_performed;
        _actionMov2.canceled += _actionMov2_canceled;
    }

    private void _frontAttackChecker_OnTriggerEnterEvent(int layer)
    {
        if (LayerMask.NameToLayer("Default") == layer ||
            LayerMask.NameToLayer("Ground") == layer)
        {
            _hitWallWhileDash = true;
        }
    }

    private void _actionMov2_performed(InputAction.CallbackContext obj)
    {
        _actionMov2Pressed = true;
    }

    private void _actionMov2_canceled(InputAction.CallbackContext obj)
    {
        _actionMov2Pressed = false;
    }

    private void Update()
    {
        if (_fuel <= 0)
            return;

        _fuelConsumed += _fuelPerSecond * Time.deltaTime;
        if (_fuelConsumed >= 1)
        {
            _fuelConsumed -= 1;
            _fuel--;
        }
        switch (_currentState)
        {
            case KlunkState.Dashing:
                if (_remainingDashTime <= 0 || _hitWallWhileDash)
                {
                    _currentState = KlunkState.None;
                    _characterController.Velocity.x = 0;
                    _characterController.RemoveFreezeConstraint(RigidbodyConstraints.FreezePositionY);
                    _frontAttack.gameObject.SetActive(false);
                    _hitWallWhileDash = false;
                    return;
                }
                _remainingDashTime -= Time.deltaTime;
                break;
            case KlunkState.Sk8erBoi:
                if (_fuel <= _sk8erBoiFuelPerSecond)
                {
                    _currentState = KlunkState.None;
                    return;
                }
                MoverHorizontal(_actionMove.ReadValue<Vector2>().normalized.x, _sk8Speed);
                if (_actionJump.triggered)
                {
                    Jump();
                }
                _fuelConsumed += _sk8erBoiFuelPerSecond * Time.deltaTime;
                break;
            default:
                MoverHorizontal(_actionMove.ReadValue<Vector2>().normalized.x, _speed);
                if (_actionMov1.triggered && _actionMove.ReadValue<Vector2>().normalized.x != 0 && _fuel >= _dashFuel)
                {
                    _characterController.Velocity.x = _actionMove.ReadValue<Vector2>().normalized.x * _dashSpeed;
                    _characterController.AddFreezeConstraint(RigidbodyConstraints.FreezePositionY);
                    _frontAttack.gameObject.SetActive(true);
                    _remainingDashTime = _dashDistance / _dashSpeed;
                    _currentState = KlunkState.Dashing;
                    _fuel -= _dashFuel;
                    return;
                }
                else if (_actionMov2Pressed && _fuel > _sk8erBoiFuelPerSecond)
                {
                    _currentState = KlunkState.Sk8erBoi;
                    return;
                }
                if (_actionJump.triggered)
                {
                    Jump();
                }
                break;
        }
    }

    void MoverHorizontal(float dir, float speed)
    {
        _characterController.Velocity.x = dir * speed;
    }

    void Jump()
    {
        _characterController.Velocity.y = _jumpHeight;
    }
}
