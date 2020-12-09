using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum RoyStates : byte
{
    Grappling,
    Grounded,
    OnAir,
    none
}

public class RoyMovementPattern : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _airSpeed;
    [SerializeField] float _grappleVelocity;
    [SerializeField] float _grappleMax;
    [SerializeField] float _jumpHeight;
    [SerializeField] float _stringForce;
    [SerializeField] float _damp;
    [SerializeField] public float _mov1Charge;
    [SerializeField] public float _mov2Charge;
    [SerializeField] LineRenderer _aimLine;
    [SerializeField] LineRenderer _hookLine;
    [SerializeField] GameObject _royBodyView;
    [SerializeField] Vector3 _groundDetectorOffset;
    [SerializeField] Vector3 _groundDetectorSize;
    [SerializeField] FloatVarSO _gasLevel;
    [SerializeField] GUIStyle _style;

    //TODO:remover essa gabiarra
    [SerializeField] int quem;

    RoyStates _actualRootState;
    InputAction _actionMove;
    InputAction _actionJump;
    InputAction _actionInteract;
    InputAction _actionBoost;
    InputAction _actionGrapple;
    InputAction _actionSupport;
    InputAction _actionOpenMap;
    InputAction _actionAim;

    Rigidbody _rb;
    SpringJoint _sprJoint;
    AimController _aimController;

    Vector2 _moveInput;
    Vector3 _aimInput;

    LayerMask _groundLayer;
    LayerMask _grappleLayer;
    
    RaycastHit _aimHitInfo;

    bool _grapplePressed;
    bool _jumpPressed;
    bool _hasGrapplePoint;
    bool _isGrapplingDone;

    bool _isBoosting;
    bool _isAiming;
    bool _facedRight;

    public float IncreaseGasLevel
    {
        set
        {
            _gasLevel.Value += value;
        }
    }

    bool IsGrounded
    {
        get
        {
            return Physics.Raycast(new Ray(transform.position, Vector3.down), _groundDetectorOffset.y);
        }
    }

    bool IsGasSufficientToGrapple
    {
        get
        {
            return _gasLevel.Value - (Vector3.Distance(transform.position, _aimHitInfo.point) / _grappleMax) > 0;
        }
    }

#if UNITY_EDITOR
    //Vars for custom editor
    [HideInInspector] public bool _groupConsume;
    [HideInInspector] public bool _groupConstants;
    [HideInInspector] public bool _gameObjects;
    [HideInInspector] public bool _debbgOptions;
    [HideInInspector] public bool _physicsSettings;
#endif

    private void Awake()
    {
        _actualRootState = RoyStates.none;

        _rb = GetComponent<Rigidbody>();
        _aimController = GetComponentInChildren<AimController>();

        _groundLayer = LayerMask.NameToLayer("Ground");
        _grappleLayer = LayerMask.NameToLayer("Grapple");

        //TODO: valor de inicialização asset global
        _gasLevel.Value = 1;
        _aimInput = Vector2.up;

        var playerInputComponent = GetComponent<PlayerInput>();
        _actionMove = playerInputComponent.actions["Move"];
        _actionJump = playerInputComponent.actions["Jump"];
        _actionInteract = playerInputComponent.actions["Interaction"];
        _actionBoost = playerInputComponent.actions["Boost"];
        _actionGrapple = playerInputComponent.actions["Grapple"];
        _actionSupport = playerInputComponent.actions["Support"];
        _actionOpenMap = playerInputComponent.actions["OpenMap"];
        _actionAim = playerInputComponent.actions["Aim"];
    }

    private void Start()
    {
        _actionMove.performed += ctx =>
        {
            _moveInput = ctx.ReadValue<Vector2>();
            if (_moveInput.x > 0)
            {
                _facedRight = true;
                _royBodyView.transform.localRotation = Quaternion.Euler(0, 120, 0);
            }
            if (_moveInput.x < 0)
            {
                _facedRight = false;
                _royBodyView.transform.localRotation = Quaternion.Euler(0, 240, 0);
            }
        };
        _actionMove.canceled += ctx => _moveInput = Vector2.zero;

        _actionJump.performed += ctx => _jumpPressed = true;
        _actionJump.canceled += ctx => _jumpPressed = false;

        _actionBoost.performed += ctx => _isBoosting = true;
        _actionBoost.canceled += ctx => _isBoosting = false;
        
        _actionGrapple.started += ctx => _grapplePressed = true;
        _actionGrapple.canceled += ctx => _grapplePressed = false;

        _actionAim.performed += ctx =>
        {
            _isAiming = true;
            _aimController.SetAim(_isAiming);
        };

        _actionAim.canceled += ctx =>
        {
            _isAiming = false;
            _aimController.SetAim(_isAiming);
            SetAimLine(false, Vector3.zero);
        };
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 250, 500), new GUIContent("GAS: " + _gasLevel.Value), _style);
    } 
#endif

    private void FixedUpdate()
    {
        _actualRootState = NextState(_actualRootState);

        Vector3 moveInput3 = new Vector3(_moveInput.x, 0);
        moveInput3 = new Vector3(_moveInput.x, 0);

        switch (_actualRootState)
        {
            case RoyStates.Grappling:

                _rb.MovePosition(_rb.position + moveInput3 * (_speed * .1f) * Time.fixedDeltaTime);

                break;

            case RoyStates.Grounded:

                _rb.MovePosition(_rb.position + moveInput3 * _speed * Time.fixedDeltaTime);

                break;

            case RoyStates.OnAir:

                if (Vector2.Dot(new Vector2(_rb.velocity.x, 0), new Vector2(_moveInput.x, 0)) < 0)
                    _rb.AddForce(new Vector3(-_rb.velocity.x, 0), ForceMode.VelocityChange);

                float inputY = (_isBoosting && _gasLevel.Value > 0 ? 1 : 0);

                _rb.MovePosition(_rb.position + moveInput3 * (_airSpeed + (_airSpeed * .3f * inputY)) * Time.fixedDeltaTime);

                _rb.AddForce(Vector3.up * (-0.5f * Physics.gravity.y * inputY * Time.deltaTime), ForceMode.VelocityChange);
                _gasLevel.Value -= (_mov1Charge / 100) * inputY * Time.fixedDeltaTime;

                break;

            default:

                _actualRootState = NextState(_actualRootState);

                break;
        }
    }

    private void Update()
    {
        _aimInput = _actionAim.ReadValue<Vector2>();
        _hasGrapplePoint = _hasGrapplePoint = Physics.Raycast(new Ray(transform.position, _aimInput.normalized), out _aimHitInfo, _grappleMax);

        if (_hasGrapplePoint)
        {
            SetAimLine(true, _aimHitInfo.point);
        }
        else
        {
            SetAimLine(false, Vector3.zero);
        }

#if UNITY_EDITOR
        if (_hasGrapplePoint)
        {
            Debug.DrawRay(transform.position, _aimInput.normalized * _aimHitInfo.distance, Color.red);
        }
#endif

        if (_isAiming)
        {
            _aimController.SetRotation(Mathf.Atan2(_aimInput.y, _aimInput.x) * Mathf.Rad2Deg);
        }

        switch (_actualRootState)
        {
            case RoyStates.Grappling:

                if (!_grapplePressed)
                {
                    SetGrapple(false, Vector3.zero);
                    SetHookLine(false, Vector3.zero);
                    return;
                }

                _gasLevel.Value -= (_mov2Charge / 100) * Time.deltaTime;

                if (_gasLevel.Value <= 0)
                {
                    SetGrapple(false, Vector3.zero);
                    SetHookLine(false, Vector3.zero);
                    return;
                }

                if (_hookLine.positionCount > 0)
                    _hookLine.SetPosition(0, transform.position);

                break;

            case RoyStates.Grounded:

                if (_actionJump.triggered)
                {
                    _rb.AddForce(
                    (Vector3.up + new Vector3(_moveInput.x, 0, 0)) * Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y),
                    ForceMode.VelocityChange);
                }

                if (_actionGrapple.triggered && _hasGrapplePoint && IsGasSufficientToGrapple)
                {
                    _gasLevel.Value -= (_mov2Charge / 100) * Time.deltaTime;
                    SetGrapple(true, _aimHitInfo.point);
                    SetHookLine(true, _aimHitInfo.point);
                }

                break;

            case RoyStates.OnAir:

                if (_actionGrapple.triggered && _hasGrapplePoint && IsGasSufficientToGrapple)
                {
                    _gasLevel.Value -= (_mov2Charge / 100) * Time.deltaTime;
                    SetGrapple(true, _aimHitInfo.point);
                    SetHookLine(true, _aimHitInfo.point);
                }

                break;
        }
    }

    void SetAimLine(bool value, Vector3 point)
    {
        if (value)
        {
            _aimLine.positionCount = 2;
            _aimLine.SetPositions(new Vector3[2] { transform.position, point });
        }
        else
        {
            _aimLine.positionCount = 0;
        }
    }

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

    void SetGrapple(bool value, Vector3 hookPoint)
    {
        if (value)
        {
            _sprJoint = gameObject.AddComponent<SpringJoint>();
            _sprJoint.autoConfigureConnectedAnchor = false;
            _sprJoint.minDistance = 2f;
            _sprJoint.connectedAnchor = hookPoint;
            _sprJoint.spring = _stringForce;
            _sprJoint.damper = _damp;

            _isGrapplingDone = false;
        }
        else
        {
            _isGrapplingDone = true;
            Destroy(_sprJoint);
        }
    }

    RoyStates NextState(RoyStates actual)
    {
        switch (actual)
        {
            case RoyStates.Grappling:

                if (_isGrapplingDone && IsGrounded)
                {
                    _isGrapplingDone = false;
                    return RoyStates.Grounded;
                }

                if (_isGrapplingDone)
                {
                    _isGrapplingDone = false;
                    return RoyStates.OnAir;
                }

                return RoyStates.Grappling;

            case RoyStates.Grounded:

                if (_grapplePressed && IsGasSufficientToGrapple && _hasGrapplePoint)
                {
                    _hasGrapplePoint = false;
                    return RoyStates.Grappling;
                }

                if (!IsGrounded)
                    return RoyStates.OnAir;

                if (_jumpPressed && IsGrounded)
                {
                    _jumpPressed = false;
                    return RoyStates.OnAir;
                }

                return RoyStates.Grounded;

            case RoyStates.OnAir:

                if (IsGrounded)
                    return RoyStates.Grounded;

                if (_grapplePressed && _hasGrapplePoint && IsGasSufficientToGrapple)
                {                    
                    _hasGrapplePoint = false;
                    return RoyStates.Grappling;
                }

                return RoyStates.OnAir;

            default:

                if (IsGrounded)
                    return RoyStates.Grounded;

                break;
        }

        return RoyStates.none;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_aimHitInfo.point, .5f);
    }
#endif
}