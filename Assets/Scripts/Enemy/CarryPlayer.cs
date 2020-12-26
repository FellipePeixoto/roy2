using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryPlayer : MonoBehaviour
{
    public bool useTriggerAsSensor = false;
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();
    public Rigidbody _rigidbody;

    Vector3 _lastPosition;
    Transform _transform;

    void Start()
    {
        _transform = transform;
        _lastPosition = _transform.position;
        _rigidbody = GetComponent<Rigidbody>();

        if (useTriggerAsSensor)
        {
            foreach (CarryPlayerSensor sensor in GetComponentsInChildren<CarryPlayerSensor>())
            {
                sensor.carrier = this;
            }
        }      
    }

    private void LateUpdate()
    {
        if (rigidbodies.Count > 0)
        {
            for (int i = 0; i < rigidbodies.Count; i++)
            {
                Rigidbody rb = rigidbodies[i];
                Vector3 velocity = (_transform.position - _lastPosition);
                rb.transform.Translate(velocity, _transform);
            }
        }

        _lastPosition = _transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            Add(rb);
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            Remove(rb);
    }

    public void Add(Rigidbody rb)
    {
        if(!rigidbodies.Contains(rb))
            rigidbodies.Add(rb);
    }

    public void Remove(Rigidbody rb)
    {
        if (rigidbodies.Contains(rb))
            rigidbodies.Remove(rb);
    }
}
