using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroupDistanceChecker : MonoBehaviour
{
    [SerializeField] TargetGroupController _targetGroupController;
    [SerializeField] Transform _roy;
    [SerializeField] Transform _klunk;
    [Space]

    [SerializeField] float _minDistance = 3f;
    [SerializeField] float _minRadius = 12f;
    [SerializeField] float _maxRadius = 75f;

    private void Reset()
    {
        _targetGroupController = GetComponent<TargetGroupController>();
    }

    void Update()
    {
        float dist = Vector3.Distance(_roy.position, _klunk.position);
        _targetGroupController.ChangeRadius(_roy, Mathf.Clamp(dist, _minRadius, _maxRadius));
        _targetGroupController.ChangeRadius(_klunk, Mathf.Clamp(dist, _minRadius, _maxRadius));
    }
}
