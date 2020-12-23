using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyCharController : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [Space]

    [Header("Is grounded configs")]
    [SerializeField] float _xOffsetGround = .99f;
    [SerializeField] float _yOffsetGround = .99f;
    [SerializeField] float _zOffsetGround = .99f;
    [SerializeField] float _maxDistanceGroundHit = .6f;
    [SerializeField] LayerMask _groundLayerMask = 1 << 8;
    [SerializeField] Color _debbugColorGround = Color.yellow;
    RaycastHit _groundHitInfo;
    public bool Grounded { get; private set; }
    [Space]

    [Header("Horizontais livres configs")]
    [SerializeField] float _xOffsetSide = .99f;
    [SerializeField] float _yOffsetSide = .99f;
    [SerializeField] float _zOffsetSide = .99f;
    [SerializeField] float _maxDistanceSideHit = .05f;
    [SerializeField] LayerMask _canGoBesidesLayerMask = (1 << 0) + (1 << 8);
    [SerializeField] Color _debbugColorSide = Color.blue;
    RaycastHit _sideHitInfo;
    public bool HasWall { get; private set; }
    [Space]

    [SerializeField] float _baseSpeed = 15;
    [SerializeField] float _jumpHeight = 6;
    [SerializeField] float _airSpeedFactor = .65f;
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
    bool _triggerHookInertia;

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
    }

    private void FixedUpdate()
    {
        if (_jump && IsGrounded() && CanJump && !_triggerHookInertia)
        {
            //_rb.AddForce(
            //        Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y),
            //        ForceMode.VelocityChange);
            _horizontalInertia = _rb.velocity.x;
            _rb.velocity += Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y);
            CanJump = false;
        }

        if (_triggerHookInertia && IsGrounded())
        {
            _triggerHookInertia = false;
        }

        if (_rb.velocity.y < 0 && !_triggerHookInertia)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_fallMultiply - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.velocity.y > 0 && !_jump && !_triggerHookInertia)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiply - 1) * Time.fixedDeltaTime;
        }

        Vector3 targetSpeed;
        if (IsGrounded())
        {
            targetSpeed = new Vector3(Velocity.x, _rb.velocity.y);
        }
        else if (_triggerHookInertia)
        {
            targetSpeed = _rb.velocity;
            targetSpeed.x = Mathf.Clamp(Velocity.x + targetSpeed.x, -_baseSpeed, _baseSpeed);
        }
        else
        {
            int resetSpeedAgainstWalss = HasWalls() ? 0 : 1;

            if (!_dontIncrementSpeed)
            {
                targetSpeed = new Vector3(_horizontalInertia *
                    resetSpeedAgainstWalss + 
                    (Velocity.x * resetSpeedAgainstWalss), 0);
            }
            else
            {
                targetSpeed = new Vector3(Velocity.x * resetSpeedAgainstWalss, 0);
            }

            if (!_ignoreHorizontalSpeedClamp)
            {
                targetSpeed = Vector3.ClampMagnitude(targetSpeed, _baseSpeed * _baseSpeedMultiplier);
            }
            targetSpeed.y = _rb.velocity.y;
        }

        _rb.velocity = targetSpeed;

        if (Velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            FacedRight = true;
        }
        else if (Velocity.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            FacedRight = false;
        }
    }

    public void Move(float dir, float speedFactor, bool jump)
    {
        if (IsGrounded() || _ignoreAirSpeed)
        {
            Velocity = Vector3.Lerp(new Vector3(dir, 0), new Vector3(dir * (_baseSpeed * speedFactor), 0), Mathf.Abs(dir));
            //Velocity.x = dir * (_baseSpeed * speedFactor);
        }
        else
        {
            Velocity.x = dir * (_baseSpeed * _airSpeedFactor);
        }
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

    public void TriggerHookInertia()
    {
        _triggerHookInertia = true;
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
            _canGoBesidesLayerMask);

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
}
