using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField] GroundChecker _groundChecker;
    [SerializeField] GroundChecker _frontCheck;
    [SerializeField] float _baseSpeed = 15;
    [SerializeField] float _jumpHeight = 6;
    [SerializeField] float _movementSmothing = .05f;
    [SerializeField] float _airSpeedFactor = .65f;
    Vector3 Velocity;
    Rigidbody _rb;
    bool _jump;
    bool _jumped;
    bool _ignoreHorizontalSpeedClamp;
    bool _ignoreSpeedSmooth;
    bool _ignoreAirSpeed;
    bool _incrementSpeed;

    public float BaseSpeed { get { return _baseSpeed; } }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 whatever = Vector3.zero;
        Vector3 targetSpeed;
        if (_groundChecker.IsGrounded())
        {
            targetSpeed = new Vector3(Velocity.x, _rb.velocity.y);
        }
        else
        {
            if (_incrementSpeed)
                targetSpeed = new Vector3(_rb.velocity.x + (Velocity.x), 0);
            else
                targetSpeed = new Vector3(Velocity.x, 0);
            if (!_ignoreHorizontalSpeedClamp)
                targetSpeed = Vector3.ClampMagnitude(targetSpeed, _baseSpeed);
            targetSpeed.y = _rb.velocity.y;
        }
        if (!_ignoreSpeedSmooth && !_frontCheck.IsGrounded())
        {
            _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetSpeed, ref whatever, _movementSmothing);
        }
        else if (!_frontCheck.IsGrounded())
            _rb.velocity = targetSpeed;


        if (Velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Velocity.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (!_jumped && _jump && _groundChecker.IsGrounded())
        {
            _rb.AddForce(
                    Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y),
                    ForceMode.VelocityChange);
            _jumped = true;
        }
        if (_jumped && !_groundChecker.IsGrounded())
        {
            _jumped = false;
        }
    }

    public void Move(float dir, float speedFactor, bool jump)
    {
        if (_groundChecker.IsGrounded() || _ignoreAirSpeed)
        {
            Velocity.x = dir * (_baseSpeed * speedFactor);
        }
        else
        {
            Velocity.x = dir * (_baseSpeed * _airSpeedFactor);
        }
        _jump = jump;
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

    public void IncrementSpeed(bool value)
    {
        _incrementSpeed = value;
    }
}
