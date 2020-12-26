using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ScavengeController : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] Vector3 _startPoint;
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
        _startPoint = transform.position;
    }

    private void Start()
    {
        if (!_startFromLeft)
        {
            _maxDistance = -_maxDistance;
            transform.position += Vector3.right * _maxDistance;
        }

        LeanTween.moveX(gameObject, transform.position.x + _maxDistance, _timerToTurn)
            .setEase(_leanType).setLoopPingPong();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
            return;

        transform.position = 
            new Vector3(_startPoint.x + (_maxDistance * _debbug_scavagePosition),
            _startPoint.y,
            _startPoint.z);
    }
#endif

    private void OnDrawGizmos()
    {
        Gizmos.color = _debbugMaxDistanceColor;
        Vector3 center = Vector3.Lerp(_startPoint, _startPoint + Vector3.right * _maxDistance, .5f);
        Gizmos.DrawCube(center, Vector3.one + Vector3.right * _maxDistance);
        if (_startFromLeft)
            Gizmos.DrawMesh(_mesh, _startPoint);
        else
            Gizmos.DrawMesh(_mesh, _startPoint + Vector3.right * _maxDistance);
    }
}
