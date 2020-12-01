using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Interactables { Door, Plate, Terminal }

public class Interctable : MonoBehaviour
{
    [SerializeField] Interactables _interactableType;

    protected bool _isInterctable;
}
