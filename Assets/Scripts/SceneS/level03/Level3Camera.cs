using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Camera : MonoBehaviour
{
    [SerializeField] float _cartSpeed = .01f;
    [SerializeField] CinemachineVirtualCamera _trackedCamera;
    [SerializeField] CineMachineController _cmController;

    CinemachineTrackedDolly _trackedDolly;

    private void Start()
    {
        _cmController = FindObjectOfType<CineMachineController>();
        _trackedDolly = _trackedCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        StartCoroutine(Scriptado());
    }

    IEnumerator Scriptado()
    {
        yield return new WaitForSeconds(0.75f);
        _cmController.ChangeToCamera("CM_ShowLevel");
        float progress = 0;
        while (_trackedDolly.m_PathPosition < 1)
        {
            progress += _cartSpeed * Time.fixedDeltaTime;
            _trackedDolly.m_PathPosition = progress;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1.2f);
        _cmController.ChangeToCamera("CM_Main");
    }
}
