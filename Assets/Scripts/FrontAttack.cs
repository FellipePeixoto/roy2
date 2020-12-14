using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TriggerEnterLayerHandler(int layer);

public class FrontAttack : MonoBehaviour
{
    public event TriggerEnterLayerHandler OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HIT");
        OnTriggerEnterEvent?.Invoke(other.gameObject.layer);
        if (other.gameObject.layer != LayerMask.NameToLayer("Weak_Wall"))
            return;

        other.gameObject.SetActive(false);
    }
}
