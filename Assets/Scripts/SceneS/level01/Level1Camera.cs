using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Camera : MonoBehaviour
{
    [SerializeField] CineMachineController _cmController;

    private void Start()
    {
        _cmController = FindObjectOfType<CineMachineController>();
        StartCoroutine(Scriptado());
    }

    IEnumerator Scriptado()
    {
        yield return new WaitForSeconds(0.75f);
        _cmController.ChangeToCamera("CM_vcamWall");
        yield return new WaitForSeconds(7);
        _cmController.ChangeToCamera("CM_Main");
        yield return new WaitUntil(() =>
        {
            return true;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}
