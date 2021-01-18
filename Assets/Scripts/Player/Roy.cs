using UnityEngine;
using UnityEngine.InputSystem;

public delegate void CurrentHandler(float percent);

public enum RoyStates : byte
{
    Hooked,
    Boosting,
    None
}

public class Roy : MonoBehaviour
{
    [Tooltip("Energia e combustível infinitos")]
    [SerializeField] bool _infinityMode = false;

    [Space]
    [Header("VITALIDADE E COMBUSTÍVEL")]
    [SerializeField] int _maxEnergy = 100;
    [SerializeField] int _maxFuel = 100;
    [Tooltip("In Seconds")]
    [SerializeField] int _energyTimeDuration = 600;
    [Tooltip("In Seconds")]
    [SerializeField] int _totalTimeToReloadFullFuel = 120;
    [SerializeField] int _collectedBatteryRechargeValue = 2;
    [SerializeField] float _energyLossPerSecond = 10f;

    [Space]
    [Header("Gancho")]
    [Tooltip("lançador do gancho, gasto por segundo")]
    [SerializeField] int _hookFuelCostPerSecond = 20;
    [SerializeField] float _hookMaxDistance = 10;
    [Tooltip("Percent")]
    [Range(0, 1)] [SerializeField] float _minDistanceFromHookPoint = 0.2f;
    [Tooltip("Percent")]
    [Range(0, 1)] [SerializeField] float _maxnDistanceFromHookPoint = 0.85f;
    [SerializeField] float _stringForce = 100;
    [SerializeField] float _damp = 20;
    [SerializeField] float _springTolerance = .025f;
    [SerializeField] LayerMask _hookLayerMask = 1 << 8;
    [SerializeField] Color _aimDebbugColorNoTarget = Color.red;
    [SerializeField] Color _aimDebbugColorTarget = Color.green;
    [SerializeField] LineRenderer _hookLine;
    [SerializeField] Magnetic _magnetic;

    [Space]
    [Header("Boost turbina")]
    [SerializeField] int _boostCostPerSecond = 10;
    [Tooltip("Velocidade vertical do boost (positiva)")]
    [SerializeField] float _boostSpeedFactor = 3;

    [Space]
    [SerializeField] RoyCharController _characterController;
    [SerializeField] Collider _mainCollider;

    RoyStates _currentState;
    InputAction _actionMove;
    InputAction _actionJump;
    InputAction _actionInteract;
    InputAction _actionMov2;
    InputAction _actionMov1;
    InputAction _actionSupport;
    InputAction _actionOpenMap;

    SpringJoint _sprJoint;
    AimController _aimController;

    int _currentEnergy;
    int _currentFuel;

    float _energyPerSecond;
    float _fuelConsumed;
    float _energyConsumed;
    float _fuelRecharged;
    float _rechargePerSecond;

    RaycastHit _aimHitInfo;

    bool _actionMovHookPressed;
    bool _actionMovBoostPressed;
    bool _jump;
    bool _isWalking, _isHovering;
    private InGameUIController inGameUIController;

    public event CurrentHandler OnCurrentEnergyChange;
    public event CurrentHandler OnCurrentFuelChange;

    private void Reset()
    {
        _characterController = GetComponent<RoyCharController>();
        _mainCollider = GetComponent<Collider>();
        _hookLine = GetComponentInChildren<LineRenderer>();
    }

    private void Awake()
    {
        _magnetic = GetComponent<Magnetic>();
        _currentState = RoyStates.None;
        _currentEnergy = _maxEnergy;
        OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
        _currentFuel = _maxFuel; 
        OnCurrentFuelChange?.Invoke((float) _currentFuel / _maxFuel);
        _energyPerSecond = (float)_maxEnergy / _energyTimeDuration;
        _rechargePerSecond = (float)_maxFuel / _totalTimeToReloadFullFuel;

        _aimController = GetComponentInChildren<AimController>();
        var playerInputComponent = GetComponent<PlayerInput>();
        _magnetic.OnPull += _onPlayerPull_Magnetic;

        _actionMove = playerInputComponent.actions["Move"];
        _actionJump = playerInputComponent.actions["Jump"];
        _actionJump.performed += _actionJump_performed;
        _actionJump.canceled += _actionJump_canceled;
        _actionMov1 = playerInputComponent.actions["Mov1"];
        _actionMov1.started += _actionMov1_started;
        _actionMov1.canceled += _actionMov1_canceled;
        _actionMov2 = playerInputComponent.actions["Mov2"];
        _actionMov2.performed += _actionMov2_performed;
        _actionMov2.canceled += _actionMov2_canceled;
        inGameUIController = FindObjectOfType<InGameUIController>();
    }

    

    private void _actionJump_performed(InputAction.CallbackContext obj)
    {
        if (inGameUIController.Paused)
            return;
        _jump = true;
    }

    private void _actionJump_canceled(InputAction.CallbackContext obj)
    {
        if (inGameUIController.Paused)
            return;
        _jump = false;
        _characterController.CanJump = true;
    }

    private void _actionMov2_canceled(InputAction.CallbackContext obj)
    {
        if (inGameUIController.Paused)
            return;
        _actionMovBoostPressed = false;
    }

    private void _actionMov2_performed(InputAction.CallbackContext obj)
    {
        if (inGameUIController.Paused)
            return;
        _actionMovBoostPressed = true;
    }

    private void _actionMov1_canceled(InputAction.CallbackContext obj)
    {
        if (inGameUIController.Paused)
            return;
        _actionMovHookPressed = false;
    }

    private void _actionMov1_started(InputAction.CallbackContext obj)
    {
        if (inGameUIController.Paused)
            return;
        _actionMovHookPressed = true;
    }

    private void _onPlayerPull_Magnetic()
    {
        if (inGameUIController.Paused)
            return;
        _energyConsumed += _energyConsumed * _energyLossPerSecond * Time.fixedDeltaTime;
    }

    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 720, 480, 360));
        GUIStyle style = GUI.skin.box;
        style.fontSize = 40;
        style.wordWrap = true;
        GUI.Box(new Rect(0, 0, 480, 45), $"ENERGY: {_currentEnergy}", style);
        GUI.Box(new Rect(0, 45, 480, 45), $"FUEL: {_currentFuel}", style);
        if (GUI.Button(new Rect(0, 90, 480, 45), "LIGAR/DESLIGAR INFINITO", style))
        {
            _infinityMode = !_infinityMode;
        }
        GUI.Box(new Rect(0, 135, 480, 45), $"INFINITO: {_infinityMode}", style);
        GUI.EndGroup();
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
        if (_currentEnergy <= 0)
            return;

        if (_currentEnergy > 0)
            _energyConsumed += _energyPerSecond * Time.fixedDeltaTime;
        if (_energyConsumed >= 1)
        {
            _energyConsumed -= 1;
            _currentEnergy = Mathf.Clamp(--_currentEnergy, 0, _maxEnergy);
            OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
        }

        if (_currentFuel < _maxFuel)
            _fuelRecharged += _rechargePerSecond * Time.fixedDeltaTime;
        if (_fuelRecharged >= 1)
        {
            _fuelRecharged -= 1;
            _currentFuel = Mathf.Clamp(++_currentFuel, 0, _maxFuel); 
            OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel); 
            OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
        }

        switch (_currentState)
        {
            case RoyStates.Hooked:
                if (_currentFuel <= 0 || !_actionMovHookPressed)
                {
                    _currentState = RoyStates.None;
                    UnHook();
                    _characterController.SetHook(false);
                    return;
                }
                _fuelConsumed += _hookFuelCostPerSecond * Time.fixedDeltaTime;
                if (_fuelConsumed >= 1)
                {
                    _fuelConsumed -= 1;
                    _currentFuel = Mathf.Clamp(--_currentFuel, 0, _maxFuel); 
                    OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
                }
                _characterController.Move(_actionMove.ReadValue<Vector2>(), 1, false);
                break;

            case RoyStates.Boosting:
                if(!_isHovering) {
                    _isHovering = true;
                    AudioManager.instance.Play("roy_hover");
                }
                if (_currentFuel <= 0 || !_actionMovBoostPressed)
                {
                    _currentState = RoyStates.None;
                    AudioManager.instance.Stop("roy_hover");
                    _isHovering = false;
                }
                _fuelConsumed += _boostCostPerSecond * Time.fixedDeltaTime;
                if (_fuelConsumed >= 1)
                {
                    _fuelConsumed -= 1;
                    _currentFuel = Mathf.Clamp(--_currentFuel, 0, _maxFuel); 
                    OnCurrentFuelChange?.Invoke((float)_currentFuel / _maxFuel);
                }
                break;

            default:
                if(!_isWalking) {
                    _isWalking = true;
                    
                    AudioManager.instance.Play("roy_walk");
                }
                _characterController.Move(_actionMove.ReadValue<Vector2>(), 1f, _jump);
                if (_actionMovHookPressed
                    && _currentFuel >= _hookFuelCostPerSecond
                    && HasHookPoint())
                {
                    _characterController.SetHook(true);
                    _currentState = RoyStates.Hooked;
                    AudioManager.instance.Stop("roy_walk");
                    _isWalking = false;
                    HookTo(_aimHitInfo.point);
                    return;
                }
                if (_actionMovBoostPressed && _currentFuel >= _boostCostPerSecond)
                {
                    _currentState = RoyStates.Boosting;
                    AudioManager.instance.Stop("roy_walk");
                    _isWalking = false;
                    return;
                }
                break;
        }
    }

    private void LateUpdate()
    {
        if (_currentState == RoyStates.Hooked)
            _hookLine.SetPosition(0, _mainCollider.bounds.center);
    }

    bool HasHookPoint()
    {
        return Physics.Raycast(
            _mainCollider.bounds.center,
            _actionMove.ReadValue<Vector2>(),
            out _aimHitInfo, _hookMaxDistance,
            _hookLayerMask);
    }

    //TODO: BRACO DO ROY!
    void SetHookLine(bool value, Vector3 point)
    {
        if (value)
        {
            _hookLine.positionCount = 2;
            _hookLine.SetPositions(new Vector3[2] { transform.position, point });
        }
        else
        {
            _hookLine.positionCount = 0;
        }
    }

    void HookTo(Vector3 hookPoint)
    {
        _sprJoint = gameObject.AddComponent<SpringJoint>();
        _sprJoint.autoConfigureConnectedAnchor = false;
        _sprJoint.enablePreprocessing = false;
        _sprJoint.minDistance = _minDistanceFromHookPoint;
        _sprJoint.maxDistance =
            Mathf.Clamp(_aimHitInfo.distance / 2, _minDistanceFromHookPoint, _hookMaxDistance);
        _sprJoint.anchor = new Vector3(0, .5f, 0);
        _sprJoint.connectedAnchor = hookPoint;
        _sprJoint.spring = _sprJoint.massScale = _stringForce;
        _sprJoint.damper = _damp;
        _hookLine.enabled = true;
        _hookLine.SetPositions(new Vector3[2]);
        _hookLine.SetPosition(1, hookPoint);
        AudioManager.instance.Play("roy_hookset");
    }

    void UnHook()
    {
        _hookLine.enabled = false;
        Destroy(_sprJoint);
        AudioManager.instance.Play("roy_hookrelease");
    }

    public void AddBattery() 
    {
        _currentEnergy = 
        Mathf.Clamp(_currentEnergy + _collectedBatteryRechargeValue, 0, _maxEnergy);
        OnCurrentEnergyChange?.Invoke((float)_currentEnergy / _maxEnergy);
    }

    private void OnDrawGizmos()
    {
        if (_actionMove == null)
            return;

        if (HasHookPoint())
        {
            Gizmos.color = _aimDebbugColorTarget;
            Gizmos.DrawRay(_mainCollider.bounds.center, _actionMove.ReadValue<Vector2>() * _aimHitInfo.distance);
        }
        else
        {
            Gizmos.color = _aimDebbugColorNoTarget;
            Gizmos.DrawRay(_mainCollider.bounds.center, _actionMove.ReadValue<Vector2>() * _hookMaxDistance);
        }
    }

    private void OnDestroy()
    {
        _magnetic.OnPull -= _onPlayerPull_Magnetic;
    }

    public Vector2 GetCurrentHpSp(){
        return new Vector2(_currentEnergy,_currentFuel);
    }
    public void SetCurrentHpSp(Vector2 stats){
        int hp = (int)stats.x;
        int sp = (int)stats.y;

        _currentEnergy = hp;
        _currentFuel = sp;
    }
}