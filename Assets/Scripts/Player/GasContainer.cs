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
    }
}
