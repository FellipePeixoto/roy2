using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;

    bool IsActive { get; set; } = true;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void GetAttracted(Vector3 force, ForceMode forceMode = ForceMode.VelocityChange)
    {
        if (!IsActive)
        {
            return;
        }

        switch (forceMode)
        {
            case ForceMode.Acceleration:
                _rb.AddForce(force, ForceMode.Acceleration);
                break;

            case ForceMode.VelocityChange:
                _rb.AddForce(force, ForceMode.VelocityChange);
                break;

            case ForceMode.Impulse:
                _rb.AddForce(force, ForceMode.Impulse);
                break;

            case ForceMode.Force:
                _rb.AddForce(force, ForceMode.Force);
                break;
        }
    }
}
