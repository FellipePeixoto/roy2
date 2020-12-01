using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalController : Interctable
{
    private void OnTriggerEnter(Collider other)
    {
        var interactor = other.GetComponent<Interactor>();

        if (interactor == null)
            return;

        if (interactor.Character != Characters.Roy)
            return;

        _isInterctable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        var interactor = other.GetComponent<Interactor>();

        if (interactor == null)
            return;

        if (interactor.Character != Characters.Roy)
            return;

        _isInterctable = false;
    }
}
