using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] Characters _character;

    public Characters Character { get => _character; }

    public void OnInteract(Interactables interctable)
    {
        switch (interctable)
        {
            case Interactables.Door:
                
                break;

            case Interactables.Plate:
                
                break;

            case Interactables.Terminal:
                
                break;
        }
    }
}
