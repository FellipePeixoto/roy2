using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PumpgasHandler();

public class GasContainer : MonoBehaviour
{
    public event PumpgasHandler OnPump;

    public void Pick(float perSecond)
    {        
        OnPump?.Invoke();
        Roy roy = GetComponent<Roy>();
        if (roy)
        {
            roy.DecreaseFuel(perSecond);
            return;
        }
        Klunk klunk = GetComponent<Klunk>();
        if (klunk)
        {
            klunk.DecreaseFuel(perSecond);
        }
    }
}
