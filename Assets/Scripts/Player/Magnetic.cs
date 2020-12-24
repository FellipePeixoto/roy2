using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    [SerializeField] bool _isActive;
    [SerializeField] Rigidbody _rb;
    public ForceMode currentForceMode;

    bool IsActive { set => _isActive = value; }

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(CalcForce() * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    public void GetAttracted(Vector3 force)
    {
        switch (currentForceMode)
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

    Vector3 GilbertForce(Magnet magnetic)
    {
        var dir = magnetic.transform.position - transform.position;
        var dist = Vector3.Distance(magnetic.transform.position, transform.position);

        var part1 = magnetic.Force;
        var part2 = 4 * Mathf.PI * dist;

        var f = part1 / part2;

        return f * dir.normalized;
    }

    Vector3 CalcForce()
    {
        var magnetics = FindObjectsOfType<Magnet>();

        if (magnetics.Length < 1)
        {
            SendMessage("OnRestoreFriction");
            return Vector3.zero;
        }

        var notIgnored = magnetics.Where((ctx) => !ctx.Ignore);

        if (notIgnored.Count() > 0)
        {
            SendMessage("OnZeroFriction");
        }
        else
        {
            SendMessage("OnRestoreFriction");
        }

        Vector3 resultantForce = Vector3.zero;

        foreach (Magnet m in notIgnored)
        {
            if(!m.Repulsive)
                resultantForce += GilbertForce(m);
            else
                resultantForce -= GilbertForce(m);
        }

        return resultantForce;
    }
}
