﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnetableController : MonoBehaviour
{
    [SerializeField] bool _isActive;    

    Rigidbody _rb;
    bool IsActive { set => _isActive = value; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }

        _rb.AddForce(CalcForce() * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    Vector3 GilbertForce(Magnetic magnetic)
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
        var magnetics = FindObjectsOfType<Magnetic>();

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

        foreach (Magnetic m in notIgnored)
        {
            if(!m.Repulsive)
                resultantForce += GilbertForce(m);
            else
                resultantForce -= GilbertForce(m);
        }

        return resultantForce;
    }
}
