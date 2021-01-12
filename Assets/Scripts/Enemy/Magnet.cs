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

        if (_attractive)
        {
            current.GetAttracted(targetDir * (_force * forceFactor));
        }
        else
        {
            current.GetRepulsed(-targetDir * (_force * forceFactor));
        }
    }

    public Magnetic GetMagnetic(int instanceID)
    {
        Magnetic m;
        if (_magnetics.TryGetValue(instanceID, out m))
        {
            return m;
        }

        return null;
    }
}
