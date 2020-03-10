using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Collectables : byte
{
    Bottle,
    Battery,
    Thrash,
    Card,
    Cereal
}

public class Collectable : MonoBehaviour
{
    [SerializeField] Collectables _collectedType;
    [SerializeField] GameObject particle;

    Collider _collider;
    MeshRenderer _meshRenderer;

    public void Awake()
    {
        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var collector = other.GetComponent<Collector>();

        if (collector)
        {
            collector.SetTypeCollected(_collectedType);
            _collider.enabled = false;
            _meshRenderer.enabled = false;
            Instantiate(particle, transform);
            Destroy(gameObject, 5);
        }
    }
}
