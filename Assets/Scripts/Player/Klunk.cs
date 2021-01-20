using System;
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
    [SerializeField] int _totalTimeToReloadFullEnergy = 240;
    [SerializeField] int _collectedBottleRechargeValue = 2;
    [SerializeField] float _energyLossPerSecond = 25f;

    [Space]
    [Header("Investida com escudo")]
    [Tooltip("investida com seu escudo energizado")]
    [SerializeField] int _dashEnergyCost = 20;
    [SerializeField] float _dashMaxDistance = 18;
    [Tooltip("Quantas vezes o dash é maior em relação a velocidade normal")]
    [SerializeField] float _dashSpeedFactor = 2;

    [Space]
    [Header("Movimentação com escudo")]
    [SerializeField] int _sk8erBoiEnergyCostPerSecond = 10;
    [Tooltip("Quantas vezes o skate é maior em relação a velocidade normal")]
    [SerializeField] float _sk8SpeedFactor = 3;
    [Space]

    [SerializeField] Magnetic _magnetic;

    KlunkState _currentState;
    FrontAttack _frontAttack;
    KlunkCharController _characterController;
    FrictionController _frictionController;

    InputAction _actionMove;
    InputAction _actionJump;
    InputAction _actionMov2;
    InputAction _actionMov1;
    InputAction _actionOpenMap;

    int _currentFuel;
    int _currentEnergy;

    public event CurrentHandler OnCurrentFuelChange;
    public event CurrentHandler OnCurrentEnergyChange;

    float _fuelPerSecond;
    float _rechargePerSecond;
    float _remainingTimeDash;
    float _fuelConsumed;
    float _energyConsumed;
    float _energyRecharged;
    bool _actionMov2Pressed;
    bool _hitWallWhileDash;
    bool _jump;

    bool _isSkating, _isDashing, _isWalking;
    Vector2 temp_startPosition;
    private InGameUIController inGameUIController;

    private void Awake()
    {
        _magnetic = GetComponent<Magnetic>();
        _magnetic.OnPull += _onPlayerPull_Magnetic;
        _currentState = KlunkState.None;
        _currentFuel = _maxFuel;
        OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
        _currentEnergy = _maxEnergy;
        OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
        _fuelPerSecond = (float)_maxFuel / _fuelTimeDuration;
        _rechargePerSecond = (float)_maxEnergy / _totalTimeToReloadFullEnergy;

        _frontAttack = GetComponentInChildren<FrontAttack>();
        _frontAttack.gameObject.SetActive(false);
        _frontAttack.OnTriggerEnterEvent += _frontAttackChecker_OnTriggerEnterEvent;
        _characterController = GetComponent<KlunkCharController>();
        _frictionController = GetComponent<FrictionController>();

        var playerInput = GetComponent<PlayerInput>();
        _actionMove = playerInput.actions["Move"];
        _actionJump = playerInput.actions["Jump"];
        _actionJump.performed += _actionJump_performed;
        _actionJump.canceled += _actionJump_canceled;
        _actionMov1 = playerInput.actions["Mov1"];
        _actionMov2 = playerInput.actions["Mov2"];
        _actionMov2.performed += _actionMov2_performed;
        _actionMov2.canceled += _actionMov2_canceled;
        inGameUIController = FindObjectOfType<InGameUIController>();
    }

    private void _onPlayerPull_Magnetic()
    {
        _energyConsumed += _energyConsumed * _energyLossPerSecond * Time.fixedDeltaTime;
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
            OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
            _currentFuel = _maxFuel;
            OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
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
            OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
        }

        if (_currentEnergy < _maxEnergy)
            _energyRecharged += _rechargePerSecond * Time.fixedDeltaTime;
        if(_energyRecharged >= 1)
        {
            _energyRecharged -= 1;
            _currentEnergy = Mathf.Clamp(++_currentEnergy, 0, _maxEnergy);
            OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
        }

        switch (_currentState)
        {
            case KlunkState.Dashing:
                _remainingTimeDash -= Time.fixedDeltaTime;
                if (!_isDashing) {
                    _isDashing = true;
                    //AudioManager.instance.Play("klunk_dash");
                } 
                if (_remainingTimeDash <= 0 || _hitWallWhileDash)
                {
                    _currentState = KlunkState.None;
                    _characterController.IgnoreAirSpeed(false);
                    _characterController.Move(Vector2.zero, 0, _jump);
                    _characterController.RemoveFreezeConstraint(RigidbodyConstraints.FreezePositionY);
                    _frontAttack.gameObject.SetActive(false);
                    _hitWallWhileDash = false;
                    _frictionController.SetMaxFriction();
                    _characterController.IgnoreHorizontalClampedSpeed(false);
                    _characterController.IgnoreSpeedSmooth(false);
                    _characterController.DontIncrementSpeed(false);
                    _isDashing = false;
                    OnEndDash?.Invoke(transform.position);
                }
                break;
            case KlunkState.Sk8erBoi:
                if (_currentEnergy <= 0 || !_actionMov2Pressed)
                {
                    _currentState = KlunkState.None;
                    _characterController.SetBaseSpeedMultiplier(1);
                    OnEndSkate?.Invoke(transform.position);
                    _isSkating = false;
                    //AudioManager.instance.Stop("klunk_skate");
                    return;
                }
                if (!_isSkating){
                    _isSkating = true;
                    //AudioManager.instance.Play("klunk_skate");
                }
                _characterController.Move(_actionMove.ReadValue<Vector2>(), _sk8SpeedFactor, _jump);
                _energyConsumed += _sk8erBoiEnergyCostPerSecond * Time.fixedDeltaTime;
                if (_energyConsumed >= 1)
                {
                    _energyConsumed -= 1;
                    _currentEnergy = Mathf.Clamp(--_currentEnergy, 0, _maxEnergy);
                    OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
                }
                break;
            default:
                _characterController.Move(_actionMove.ReadValue<Vector2>(), 1, _jump);
                if(!_isWalking){
                    _isWalking = true;
                    //AudioManager.instance.Play("klunk_walk");
                }
                if (_actionMov1.triggered && _currentEnergy >= _dashEnergyCost)
                {
                    _characterController.IgnoreAirSpeed(true);
                    if (_characterController.FacedRight)
                        _characterController.Move(Vector2.right, _dashSpeedFactor, _jump);
                    else
                        _characterController.Move(-Vector2.right, _dashSpeedFactor, _jump);
                    _characterController.AddFreezeConstraint(RigidbodyConstraints.FreezePositionY);
                    _frontAttack.gameObject.SetActive(true);
                    _remainingTimeDash = _dashMaxDistance / (_characterController.BaseSpeed * _dashSpeedFactor);
                    _remainingTimeDash -= Time.fixedDeltaTime;
                    _currentState = KlunkState.Dashing;
                    _currentEnergy = Mathf.Clamp(_currentEnergy - _dashEnergyCost, 0, _maxEnergy);
                    OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
                    _frictionController.SetZeroFriction();
                    _characterController.IgnoreHorizontalClampedSpeed(true);
                    _characterController.IgnoreSpeedSmooth(true);
                    _characterController.DontIncrementSpeed(true);
                    OnStartDash?.Invoke(transform.position);
                    temp_startPosition = transform.position;
                    _isWalking = false;
                    //AudioManager.instance.Play("klunk_walk");
                    return;
                }
                else if (_actionMov2Pressed && _currentEnergy > _sk8erBoiEnergyCostPerSecond)
                {
                    _currentState = KlunkState.Sk8erBoi;
                    _characterController.SetBaseSpeedMultiplier(_sk8SpeedFactor);
                    OnStartSkate?.Invoke(transform.position);
                    _isWalking = false;
                    //AudioManager.instance.Play("klunk_walk");
                    return;
                }
                break;
        }
    }

    public void AddFuel()
    {
        _currentFuel = Mathf.Clamp(_currentFuel + _collectedBottleRechargeValue, 0, _maxFuel);
        OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
        //AudioManager.instance.Play("klunk_fuelrecharge");
    }

    public void DecreaseFuel(float perSecond)
    {
        _fuelConsumed += perSecond * Time.fixedDeltaTime;
    }

    private void OnGUI()
    {
        return;
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

    public Vector2 GetCurrentHpSp(){
        return new Vector2(_currentFuel,_currentEnergy);
    }

    public void SetCurrentHpSp(Vector2 stats){
        int hp = (int)stats.x;
        int sp = (int)stats.y;

        _currentFuel = hp;
        _currentEnergy = sp;
    }
}
