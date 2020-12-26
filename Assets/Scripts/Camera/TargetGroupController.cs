using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static Cinemachine.CinemachineTargetGroup;

public class TargetGroupController : MonoBehaviour
{
    [SerializeField] CinemachineTargetGroup _targetGroup;

    private void Reset()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    public void AddToGroup(Transform targetTransform, float weight = 1, float radius = 1)
    {
        _targetGroup.AddMember(targetTransform, weight, radius);
    }

    public void RemoveFromGroup(Transform targetTransform)
    {
        _targetGroup.RemoveMember(targetTransform);
    }

    public void ChangeRadius(Transform targetTrasnform, float newRadius)
    {
        int index = _targetGroup.FindMember(targetTrasnform);
        _targetGroup.m_Targets[index].radius = newRadius;
    }

    public void ChangeWeight(Transform targetTrasnform, float newWeight)
    {
        int index = _targetGroup.FindMember(targetTrasnform);
        _targetGroup.m_Targets[index].weight = newWeight;
    }
}
