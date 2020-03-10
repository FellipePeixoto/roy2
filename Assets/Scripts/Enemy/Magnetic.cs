using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    [SerializeField] float _force;
    [SerializeField] bool _repulsive;

    bool _ignore;
    public bool Ignore { get => _ignore; }

    public float Force { get => _force; }
    public bool Repulsive { get => _repulsive; }

    private void Awake()
    {
        _ignore = true;
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
