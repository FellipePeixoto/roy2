using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void MagneticHandler();
public delegate void MagneticHandlerPush(Vector3 force);

public class Magnetic : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [Space]

    [Header("Caso seja repelido")]
    [SerializeField] float _effectDuration = .75f;

    public event MagneticHandler OnPull;
    public event MagneticHandlerPush OnPush;

    bool IsActive { get; set; } = true;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    float _currentTime;

    public void GetAttracted(Vector3 force, ForceMode forceMode = ForceMode.VelocityChange)
    {
        if (!IsActive)
        {
            return;
        }

        OnPull?.Invoke();

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

    public void GetRepulsed(Vector3 force, ForceMode forceMode = ForceMode.VelocityChange)
    {
        if (!IsActive)
        {
            return;
        }

        OnPush?.Invoke(force);

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

    IEnumerator ImpulseEffect(Vector3 force, ForceMode forceMode = ForceMode.VelocityChange)
    {
        _currentTime = _effectDuration;
        while (_currentTime > 0)
        {
            

            _currentTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _currentTime = 0;
    }
}
