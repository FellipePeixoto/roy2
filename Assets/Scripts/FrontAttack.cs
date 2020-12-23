using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TriggerEnterLayerHandler(int layer);

public class FrontAttack : MonoBehaviour
{
    public event TriggerEnterLayerHandler OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other.gameObject.layer);

        if (other.gameObject.GetComponent<WeakWallController>())
        {
            other.gameObject.GetComponent<WeakWallController>().BreakWall();
        }
    }
}
