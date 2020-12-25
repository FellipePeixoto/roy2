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

    [SerializeField] bool _startFromLeftToRight = true;
    [SerializeField] float _speed = 12;
    [SerializeField] float _maxDistance = 14;
    [SerializeField] Color _debbugMaxDistanceColor = Color.yellow + new Color(0, 0, 0, .35f);
    [Range(0, 1)] [SerializeField] float _debbug_scavagePosition;

    bool _goingRight = true;
    float _timerToTurn;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        _startPoint = transform.position;
    }

    private void Awake()
    {
        if (!_startFromLeftToRight)
        {
            _goingRight = !_goingRight;
            transform.position = 
                new Vector3(_startPoint.x + _maxDistance,
                _startPoint.y,
                _startPoint.z);
        }
        else
        {
            transform.position = _startPoint;
        }

        _timerToTurn = _maxDistance / _speed;
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

    private void FixedUpdate()
    {
        _timerToTurn -= Time.fixedDeltaTime;
        if (_timerToTurn <= 0)
        {
            _goingRight = !_goingRight;
            _timerToTurn = _maxDistance / _speed;
            return;
        }

        float currentDirection = _goingRight ? 1 : -1;
        _rb.MovePosition(_rb.position + (((currentDirection * Vector3.right) * 12) * Time.fixedDeltaTime));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _debbugMaxDistanceColor;
        Vector3 center = Vector3.Lerp(_startPoint, _startPoint + Vector3.right * _maxDistance, .5f);
        Gizmos.DrawCube(center, Vector3.one + Vector3.right * _maxDistance);
        if (_startFromLeftToRight)
            Gizmos.DrawMesh(_mesh, _startPoint);
        else
            Gizmos.DrawMesh(_mesh, _startPoint + Vector3.right * _maxDistance);
    }
}
