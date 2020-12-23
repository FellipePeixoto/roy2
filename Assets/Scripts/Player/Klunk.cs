using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void DashStartHandler(Vector3 startPoint);
public delegate void DashEndHandler(Vector3 endPoint);

public delegate void SkateStartHandler(Vector3 startPoint);
public delegate void SkateEndHandler(Vector3 startPoint);

enum KlunkState
{
    None,
    Dashing,
    Sk8erBoi
}

public class Klunk : MonoBehaviour
{
    public event DashStartHandler OnStartDash;
    public event DashStartHandler OnEndDash;

    public event SkateStartHandler OnStartSkate;
    public event SkateStartHandler OnEndSkate;

    [Tooltip("Combustível e energia infinitos")]
    [SerializeField] bool _infinityMode = false;

    [Space]
    [Header("VITALIDADE E ENERGIA")]
    [SerializeField] int _maxFuel = 100;
    [SerializeField] int _maxEnergy = 100;
    [Tooltip("In Seconds")]
    [SerializeField] int _fuelTimeDuration = 600;
    [Tooltip("In Seconds")]
    [SerializeField] int _totalTimeToReloadFullEnergy = 120;

    [Space]
    [Header("Investida com escudo")]
    [Tooltip("investida com seu escudo energizado")]
    [SerializeField] int _dashEnergyCost = 20;
    [SerializeField] float _dashMaxDistance = 10;
    [Tooltip("Quantas vezes o dash é maior em relação a velocidade normal")]
    [SerializeField] float _dashSpeedFactor = 2;

    [Space]
    [Header("Movimentação com escudo")]
    [SerializeField] int _sk8erBoiEnergyCostPerSecond = 10;
    [Tooltip("Quantas vezes o skate é maior em relação a velocidade normal")]
    [SerializeField] float _sk8SpeedFactor = 3;

    KlunkState _currentState;
    FrontAttack _frontAttack;
    CharacterController _characterController;
    FrictionController _frictionController;

    public bool ActionsLocked { get; private set; }

    public InputAction ActionMove { get; private set; }
    public InputAction ActionJump { get; private set; }
    public InputAction ActionInteract { get; private set; }
    public InputAction ActionSupport { get; private set; }
    public InputAction ActionMovSkate { get; private set; }
    public InputAction ActionMovDash { get; private set; }
    public InputAction ActionOpenMap { get; private set; }

    int _currentFuel;
    int _currentEnergy;

    float _fuelPerSecond;
    float _rechargePerSecond;
    float _remainingTimeDash;
    float _fuelConsumed;
    float _energyConsumed;
    float _energyRecharged;
    bool _actionMov2Pressed;
    bool _hitWallWhileDash;
    bool _jump; 

    Vector3 delete_start;

    private void Awake()
    {
        _currentState = KlunkState.None;
        _currentFuel = _maxFuel;
        _currentEnergy = _maxEnergy;
        _fuelPerSecond = (float)_maxFuel / _fuelTimeDuration;
        _rechargePerSecond = (float)_maxEnergy / _totalTimeToReloadFullEnergy;

        _frontAttack = GetComponentInChildren<FrontAttack>();
        _frontAttack.gameObject.SetActive(false);
        _frontAttack.OnTriggerEnterEvent += _frontAttackChecker_OnTriggerEnterEvent;
        _characterController = GetComponent<CharacterController>();
        _frictionController = GetComponent<FrictionController>();

        var playerInput = GetComponent<PlayerInput>();
        ActionMove = playerInput.actions["Move"];
        ActionJump = playerInput.actions["Jump"];
        ActionJump.performed += _actionJump_performed;
        ActionJump.canceled += _actionJump_canceled;
        ActionInteract = playerInput.actions["Interaction"];
        ActionSupport = playerInput.actions["Support"];
        ActionMovDash = playerInput.actions["Mov1"];
        ActionMovSkate = playerInput.actions["Mov2"];
        ActionOpenMap = playerInput.actions["OpenMap"];
        ActionMovSkate.performed += _actionMov2_performed;
        ActionMovSkate.canceled += _actionMov2_canceled;
    }

    private void _actionJump_performed(InputAction.CallbackContext obj)
    {
        _jump = true;
    }

    private void _actionJump_canceled(InputAction.CallbackContext obj)
    {
        _jump = false;
        _characterController.CanJump = true;
    }

    private void _frontAttackChecker_OnTriggerEnterEvent(int layer)
    {
        if (LayerMask.NameToLayer("Ground") == layer)
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

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (_infinityMode)
        {
            _currentEnergy = _maxEnergy;
            _currentFuel = _maxFuel;
        }
#endif
        if (_currentFuel <= 0)
            return;

        if (_currentFuel > 0)
            _fuelConsumed += _fuelPerSecond * Time.fixedDeltaTime;
        if (_fuelConsumed >= 1)
        {
            _fuelConsumed -= 1;
            _currentFuel = Mathf.Clamp(--_currentFuel, 0, _maxFuel);
        }

        if (_currentEnergy < _maxEnergy)
            _energyRecharged += _rechargePerSecond * Time.fixedDeltaTime;
        if(_energyRecharged >= 1)
        {
            _energyRecharged -= 1;
            _currentEnergy = Mathf.Clamp(++_currentEnergy, 0, _maxEnergy);
        }

        if (ActionsLocked)
            return;

        switch (_currentState)
        {
            case KlunkState.Dashing:
                if (_remainingTimeDash <= 0 || _hitWallWhileDash)
                {
                    _currentState = KlunkState.None;
                    _characterController.IgnoreAirSpeed(false);
                    _characterController.Move(0, 0, _jump);
                    _characterController.RemoveFreezeConstraint(RigidbodyConstraints.FreezePositionY);
                    _frontAttack.gameObject.SetActive(false);
                    _hitWallWhileDash = false;
                    _frictionController.SetMaxFriction();
                    _characterController.IgnoreHorizontalClampedSpeed(false);
                    _characterController.IgnoreSpeedSmooth(false);
                    _characterController.DontIncrementSpeed(false);
                    OnEndDash?.Invoke(transform.position);
                }
                _remainingTimeDash -= Time.fixedDeltaTime;
                break;
            case KlunkState.Sk8erBoi:
                if (_currentEnergy <= 0 || !_actionMov2Pressed)
                {
                    _currentState = KlunkState.None;
                    _characterController.SetBaseSpeedMultiplier(1);
                    OnEndSkate?.Invoke(transform.position);
                    return;
                }
                _characterController.Move(ActionMove.ReadValue<Vector2>().x, 
                    Mathf.Abs(ActionMove.ReadValue<Vector2>().x) * _sk8SpeedFactor, _jump);
                _energyConsumed += _sk8erBoiEnergyCostPerSecond * Time.fixedDeltaTime;
                if (_energyConsumed >= 1)
                {
                    _energyConsumed -= 1;
                    _currentEnergy = Mathf.Clamp(--_currentEnergy, 0, _maxEnergy);
                }
                break;
            default:
                _characterController.Move(ActionMove.ReadValue<Vector2>().x, Mathf.Abs(ActionMove.ReadValue<Vector2>().x), _jump);
                if (ActionMovDash.triggered && _currentEnergy >= _dashEnergyCost)
                {
                    _characterController.IgnoreAirSpeed(true);
                    if (_characterController.FacedRight)
                        _characterController.Move(1, _dashSpeedFactor, _jump);
                    else
                        _characterController.Move(-1, _dashSpeedFactor, _jump);
                    _characterController.AddFreezeConstraint(RigidbodyConstraints.FreezePositionY);
                    _frontAttack.gameObject.SetActive(true);
                    _remainingTimeDash = _dashMaxDistance / (_characterController.BaseSpeed * _dashSpeedFactor);
                    _remainingTimeDash -= Time.fixedDeltaTime;
                    _currentState = KlunkState.Dashing;
                    _currentEnergy = Mathf.Clamp(_currentEnergy - _dashEnergyCost, 0, _maxEnergy);
                    _frictionController.SetZeroFriction();
                    _characterController.IgnoreHorizontalClampedSpeed(true);
                    _characterController.IgnoreSpeedSmooth(true);
                    _characterController.DontIncrementSpeed(true);
                    delete_start = transform.position;
                    OnStartDash?.Invoke(transform.position);
                    return;
                }
                else if (_actionMov2Pressed && _currentEnergy > 0)
                {
                    _currentState = KlunkState.Sk8erBoi;
                    _characterController.SetBaseSpeedMultiplier(_sk8SpeedFactor);
                    OnStartSkate?.Invoke(transform.position);
                    return;
                }
                break;
        }
    }

    public void LockActions(bool value)
    {
        ActionsLocked = value;
    }

    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(1440, 720, 480, 360));
        GUIStyle style = GUI.skin.box;
        style.fontSize = 40;
        style.wordWrap = true;
        GUI.Box(new Rect(0, 0, 480, 45), $"FUEL: {_currentFuel}", style);
        GUI.Box(new Rect(0, 45, 480, 45), $"ENERGY: {_currentEnergy}", style);
        if (GUI.Button(new Rect(0, 90, 480, 45), "LIGAR/DESLIGAR INFINITO", style))
        {
            _infinityMode = !_infinityMode;
        }
        GUI.Box(new Rect(0, 135, 480, 45), $"INFINITO: {_infinityMode}", style);
        GUI.EndGroup();
    }
}
