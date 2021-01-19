using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineMachineController : MonoBehaviour
{
    [SerializeField] GameObject _camsRoot;
    CinemachineBrain _cmBrain;
    CinemachineVirtualCamera _currentCamera;

    Dictionary<string, CinemachineVirtualCamera> _cams = 
        new Dictionary<string, CinemachineVirtualCamera>();

    private void Awake()
    {
        _cmBrain = GetComponent<CinemachineBrain>();
        CinemachineVirtualCamera[] childObjs = 
            _camsRoot.GetComponentsInChildren<CinemachineVirtualCamera>(true);
        int priority = -100;
        foreach (CinemachineVirtualCamera vcam in childObjs)
        {
            _cams.Add(vcam.name, vcam);
            if (vcam.Priority > priority)
            {
                _currentCamera = vcam;
                priority = vcam.Priority;
            }
        }
    }

    public void ChangeToCamera(string camera)
    {
        if (!_cams.ContainsKey(camera))
            return;
        _currentCamera.Priority = -1;
        _cams[camera].Priority = 1;
        _currentCamera = _cams[camera];
    }
}
