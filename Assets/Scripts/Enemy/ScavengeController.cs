using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengeController : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] GameObject _scavenger;
    [SerializeField] Mesh _mesh;
    [Space]

    [SerializeField] LeanTweenType _leanType = LeanTweenType.easeInOutSine;
    [SerializeField] bool _startFromLeft = true;
    [SerializeField] float _maxDistance = 14;
    [SerializeField] Color _debbugMaxDistanceColor = Color.yellow + new Color(0, 0, 0, .35f);
    [Range(0, 1)] [SerializeField] float _debbug_scavagePosition;

    bool _goingRight = true;
    [SerializeField] float _timerToTurn = 2;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (_startFromLeft)
        {
            _scavenger.transform.localPosition = new Vector3(0, 0);

            LeanTween.moveLocalX(_scavenger, _maxDistance, _timerToTurn)
                .setEase(_leanType).setLoopPingPong();
        }
        else
        {
            _scavenger.transform.localPosition = new Vector3(_maxDistance, 0);

            LeanTween.moveLocalX(_scavenger, 0, _timerToTurn)
                .setEase(_leanType).setLoopPingPong();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _debbugMaxDistanceColor;
        Vector3 center = Vector3.Lerp(transform.position, transform.position + Vector3.right * _maxDistance, .5f);
        Gizmos.DrawCube(center, Vector3.one + Vector3.right * _maxDistance);
        if (_startFromLeft)
            Gizmos.DrawMesh(_mesh, transform.position);
        else
            Gizmos.DrawMesh(_mesh, transform.position + Vector3.right * _maxDistance);
    }
}
