using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasRobber : MonoBehaviour
{
    [SerializeField] float perSecond = 15;

    Dictionary<int,GasContainer> _gasContainers = new Dictionary<int, GasContainer>();

    private void Awake()
    {
        GasContainer[] containers = FindObjectsOfType<GasContainer>();
        foreach(GasContainer g in containers)
        {
            _gasContainers.Add(g.transform.GetInstanceID(), g);
        }
    }

    public void TryToStealGas(int instanceID)
    {
        GasContainer current;
        if (!_gasContainers.TryGetValue(instanceID, out current))
            return;

        current.Pick(perSecond);
    }
}
