using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KlunkStates : byte 
{
    Skateee,
    Shield,
    Grounded,
    OnAir,
    none = 0
}


public class KlunkMovementPattern : MonoBehaviour
{

    KlunkStates _actualRootState;

    private void Awake()
    {
        _actualRootState = KlunkStates.none;
    }

    private void FixedUpdate()
    {
        switch (_actualRootState)
        {
            case KlunkStates.Grounded:

                break;

            case KlunkStates.OnAir:

                break;

            case KlunkStates.Skateee:

                break;

            default:

                break;
        }
    }

    void Update()
    {
        switch (_actualRootState)
        {
            case KlunkStates.Grounded:

                break;

            case KlunkStates.OnAir:

                break;

            case KlunkStates.Skateee:

                break;

            default:

                break;
        }
    }

    KlunkStates NextState()
    {


        return KlunkStates.none;
    }
}
