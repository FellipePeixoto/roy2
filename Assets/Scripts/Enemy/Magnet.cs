using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [Space]

    [SerializeField] float _force = 250;
    [SerializeField] bool _attractive = true;

    Dictionary<int, Magnetic> _magnetics = new Dictionary<int, Magnetic>();

    bool _ignore;
    public bool Ignore { get => _ignore; }

    public float Force { get => _force; }
    public bool Repulsive { get => _attractive; }

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
    }

    private void Awake()
    {
        _ignore = true;

        //TODO: CENTRALIZAR NO MANAGER
        Magnetic[] magneticsFound = FindObjectsOfType<Magnetic>();
        foreach (Magnetic item in magneticsFound)
        {
            _magnetics.Add(item.transform.GetInstanceID(), item);
        }
    }

    public void TryAttractMagnetic(int instanceID)
    {
        Magnetic current;
        if (!_magnetics.TryGetValue(instanceID, out current))
            return;

        Vector3 targetDir = (_mainCollider.bounds.center - current.transform.position).normalized;

        if (_attractive)
            current.GetAttracted(targetDir * _force);
        else
            current.GetAttracted(-targetDir * _force);
    }

    private void OnTriggerEnter(Collider other)
    {
        _ignore = !(other.tag == "Player");
    }

    private void OnTriggerExit(Collider other)
    {
        _ignore = other.tag == "Player";
    }
}
