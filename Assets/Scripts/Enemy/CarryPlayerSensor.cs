using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryPlayerSensor : MonoBehaviour
{
    [HideInInspector] public CarryPlayer carrier;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && rb != carrier._rigidbody)
        {
            carrier.Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && rb != carrier._rigidbody)
        {
            carrier.Remove(rb);
        }
    }
}
