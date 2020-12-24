using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] float _force;
    [SerializeField] bool _repulsive;

    Dictionary<int, Magnetic> _magnetics = new Dictionary<int, Magnetic>();

    bool _ignore;
    public bool Ignore { get => _ignore; }

    public float Force { get => _force; }
    public bool Repulsive { get => _repulsive; }

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

        current.GetAttracted(Vector3.one);
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
