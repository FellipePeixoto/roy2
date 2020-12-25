using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [Space]

    [SerializeField] float _force = 65;
    [SerializeField] bool _attractive = true;
    [SerializeField] ForceMode forceMode = ForceMode.VelocityChange;

    Dictionary<int, Magnetic> _magnetics = new Dictionary<int, Magnetic>();

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
    }

    private void Awake()
    {
        //TODO: CENTRALIZAR NO MANAGER
        Magnetic[] magneticsFound = FindObjectsOfType<Magnetic>();
        foreach (Magnetic item in magneticsFound)
        {
            _magnetics.Add(item.transform.GetInstanceID(), item);
        }
    }

    public void TryAttractMagnetic(int instanceID, float forceFactor = 1)
    {
        Magnetic current;
        if (!_magnetics.TryGetValue(instanceID, out current))
            return;

        forceFactor = Mathf.Clamp(forceFactor, 0, 1);

        Vector3 targetDir = (_mainCollider.bounds.center - current.transform.position).normalized;

        float forceProduct = _force;
        float distance = Vector3.Distance(_mainCollider.transform.position, current.transform.position);

        //if (distance <= _radiusWeakForce)
        //{
        //    forceProduct *= (1 - (distance / _radiusWeakForce));
        //}

        //if (distance <= _radiusStrongForce)
        //{
        //    forceProduct *= ((1 - (distance / _radiusWeakForce)) * 1.25f);
        //}

        if (_attractive)
            current.GetAttracted(targetDir * forceProduct);
        else
            current.GetAttracted(-targetDir * forceProduct);
    }
}
