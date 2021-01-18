using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KlunkCharController : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [Space]

    [Header("Is grounded configs")]
    [SerializeField] float _xOffsetGround = .99f;
    [SerializeField] float _yOffsetGround = .99f;
    [SerializeField] float _zOffsetGround = .99f;
    [SerializeField] float _maxDistanceGroundHit = .05f;
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] Color _debbugColorGround = Color.yellow;
    RaycastHit _groundHitInfo;
    public bool Grounded { get; private set; }
    [Space]

    [Header("Horizontais livres configs")]
    [SerializeField] float _xOffsetSide = .99f;
    [SerializeField] float _yOffsetSide = .99f;
    [SerializeField] float _zOffsetSide = .99f;
    [SerializeField] float _maxDistanceSideHit = .05f;
    [SerializeField] LayerMask _SideLayerMask;
    [SerializeField] Color _debbugColorSide = Color.blue;
    RaycastHit _sideHitInfo;
    public bool HasWall { get; private set; }
    [Space]

    [SerializeField] float _baseSpeed = 15;
    [SerializeField] float _jumpHeight = 6;
    [SerializeField] float _fallMultiply = 2.5f;
    [SerializeField] float _lowJumpMultiply = 2f;
    Vector3 Velocity;
    Rigidbody _rb;
    float _baseSpeedMultiplier = 1;
    float _horizontalInertia;
    bool _jump;
    bool _ignoreHorizontalSpeedClamp;
    bool _ignoreSpeedSmooth;
    bool _ignoreAirSpeed;
    bool _dontIncrementSpeed;
    [SerializeField] Animator _animator;

    public bool CanJump { get; set; } = true;

    public float BaseSpeed { get { return _baseSpeed; } }
    public bool FacedRight { get; private set; } = true;

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator.Play("Klunk_Idle");
    }

    private void FixedUpdate()
    {
        if (_jump && IsGrounded() && CanJump)
        {
            _animator.Play("Klunk_Jump_in");
            _horizontalInertia = _rb.velocity.x;
            _rb.velocity += Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y);
            CanJump = false;
            //AudioManager.instance.Play("klunk_jump");
        }

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_fallMultiply - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.velocity.y > 0 && !_jump)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiply - 1) * Time.fixedDeltaTime;
        }

        Vector3 targetSpeed;
        int resetSpeedAgainstWalss = HasWalls() ? 0 : 1;

        targetSpeed = new Vector3(Velocity.x * resetSpeedAgainstWalss, 0);
        targetSpeed.y = _rb.velocity.y;

        _rb.velocity = targetSpeed;

        if (Velocity.x > 0)
        {
            _animator.Play("Klunk_Run");
            transform.rotation = Quaternion.Euler(0, 0, 0);
            FacedRight = true;
        }
        else if (Velocity.x < 0)
        {
            _animator.Play("Klunk_Run");
            transform.rotation = Quaternion.Euler(0, 180, 0);
            FacedRight = false;
        }
        else
        {
            _animator.Play("Klunk_Idle");
        }
    }

    public void Move(Vector2 dir, float speedFactor, bool jump)
    {

        if (dir.x > 0)
            dir.x = 1;
        else if (dir.x < 0)
            dir.x = -1;

        Velocity.x = dir.x * (_baseSpeed * speedFactor);
        
        _jump = jump;
    }

    public void SetBaseSpeedMultiplier(float value)
    {
        _baseSpeedMultiplier = value;
    }

    public void AddFreezeConstraint(RigidbodyConstraints value)
    {
        _rb.constraints |= value;
    }
    public void RemoveFreezeConstraint(RigidbodyConstraints value)
    {
        _rb.constraints -= value;
    }

    public void IgnoreAirSpeed(bool value)
    {
        _ignoreAirSpeed = value;
    }

    public void IgnoreHorizontalClampedSpeed(bool value)
    {
        _ignoreHorizontalSpeedClamp = value;
    }

    public void IgnoreSpeedSmooth(bool value)
    {
        _ignoreSpeedSmooth = value;
    }

    public void DontIncrementSpeed(bool value)
    {
        _dontIncrementSpeed = value;
    }

    bool IsGrounded()
    {
        Vector3 _vectorOffset = new Vector3(_mainCollider.bounds.size.x * _xOffsetGround, 
            _mainCollider.bounds.size.y * _yOffsetGround, 
            _mainCollider.bounds.size.z * _zOffsetGround);

        Grounded = Physics.SphereCast(
            _mainCollider.bounds.center, 
            _vectorOffset.x / 2, 
            -transform.up, 
            out _groundHitInfo, 
            _maxDistanceGroundHit, 
            _groundLayerMask);

        //Grounded = Physics.BoxCast
        //    (_mainCollider.bounds.center, 
        //    _vectorOffset / 2,
        //    -transform.up, 
        //    out _groundHitInfo, 
        //    transform.rotation, 
        //    _maxDistanceGroundHit,
        //    _groundLayerMask);

        return Grounded;
    }

    bool HasWalls()
    {
        Vector3 _vectorOffset = new Vector3(_mainCollider.bounds.size.x * _xOffsetSide,
            _mainCollider.bounds.size.y * _yOffsetSide,
            _mainCollider.bounds.size.z * _zOffsetSide);

        HasWall = Physics.BoxCast
            (_mainCollider.bounds.center,
            _vectorOffset / 2,
            transform.right,
            out _sideHitInfo,
            transform.rotation,
            _maxDistanceSideHit,
            _SideLayerMask);

        return HasWall;
    }

    void DrawGroundChecker()
    {
        Gizmos.color = _debbugColorGround;

        Vector3 _vectorOffset = new Vector3(_mainCollider.bounds.size.x * _xOffsetGround,
            _mainCollider.bounds.size.y * _yOffsetGround,
            _mainCollider.bounds.size.z * _zOffsetGround);

        //Check if there has been a hit yet
        if (IsGrounded())
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(_mainCollider.bounds.center, -transform.up * _groundHitInfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireSphere(_mainCollider.bounds.center + (-transform.up * _groundHitInfo.distance), _vectorOffset.x / 2);
            //Gizmos.DrawWireCube(_mainCollider.bounds.center + -transform.up * _groundHitInfo.distance, _vectorOffset);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(_mainCollider.bounds.center, -transform.up * _maxDistanceGroundHit);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireSphere(_mainCollider.bounds.center + (-transform.up * _maxDistanceGroundHit), _vectorOffset.x / 2);
            //Gizmos.DrawWireCube(_mainCollider.bounds.center + -transform.up * _maxDistanceGroundHit, _vectorOffset);
        }
    }

    void DrawSidesChecker()
    {
        Gizmos.color = _debbugColorSide;

        Vector3 _vectorOffset = new Vector3(_mainCollider.bounds.size.x * _xOffsetSide,
            _mainCollider.bounds.size.y * _yOffsetSide,
            _mainCollider.bounds.size.z * _zOffsetSide);

        //Check if there has been a hit yet
        if (HasWalls())
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(_mainCollider.bounds.center, transform.right * _sideHitInfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(_mainCollider.bounds.center + transform.right * _sideHitInfo.distance, _vectorOffset);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(_mainCollider.bounds.center, transform.right * _maxDistanceSideHit);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(_mainCollider.bounds.center + transform.right * _maxDistanceSideHit, _vectorOffset);
        }
    }

    void OnDrawGizmos()
    {
        DrawGroundChecker();
        DrawSidesChecker();
    }

    private void OnCollisionEnter(Collision other) {
        if(other.contacts[0].normal == Vector3.up){
            //AudioManager.instance.Play("klunk_landing");
        }
    }
}