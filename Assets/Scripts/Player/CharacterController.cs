using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [HideInInspector] public Vector3 Velocity;
    GroundChecker _groundChecker;
    Rigidbody _rb;
    bool _canJump;

    private void Awake()
    {
        _groundChecker = GetComponentInChildren<GroundChecker>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector3(Velocity.x, _rb.velocity.y);
        if (_rb.velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (_rb.velocity.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Velocity.y > 0 && _canJump && _groundChecker.IsGrounded())
        {
            _rb.AddForce(
                    Vector3.up * Mathf.Sqrt(Velocity.y * -2f * Physics.gravity.y),
                    ForceMode.VelocityChange);
            _canJump = false;
            Velocity.y = 0;
        }

        if (!_canJump && !_groundChecker.IsGrounded())
        {
            _canJump = true;
        }
    }

    public void AddFreezeConstraint(RigidbodyConstraints value)
    {
        _rb.constraints |= value;
    }
    public void RemoveFreezeConstraint(RigidbodyConstraints value)
    {
        _rb.constraints -= value;
    }
}
