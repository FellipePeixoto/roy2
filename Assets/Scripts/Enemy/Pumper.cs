using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PumperState { RestPosition, GoingUp, OnTop, GoingDown }

public class Pumper : MonoBehaviour
{
    [SerializeField] Transform _pumper;
    [SerializeField] Transform _restPosition;
    [SerializeField] Transform _upperPosition;
    [SerializeField] LeanTweenType _easetypeGoUp;
    [SerializeField] LeanTweenType _easetypeGoDown;
    [SerializeField] float _timeToComplete;
    [SerializeField] Vector3 _scanBoxPlatOffest;
    [SerializeField] Vector3 _scanBoxPlatHalfExtends;
    [SerializeField] Vector3 _scanBoxHalfExtends = new Vector3(.5f, 1, .5f);
    [SerializeField] Vector3 _offsetFromCenter;
    [SerializeField] Color _scanBoxDebbugColor = Color.red;
    [SerializeField] LayerMask _whereIsGas = (1 << 0);
    [SerializeField] GasRobber _gasRobber;

    PumperState _currentState = PumperState.RestPosition;
    LTDescr _currentTween;

    [SerializeField] Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Collider[] cols = CastSides();
        if (cols.Length > 0)
        {
            switch (_currentState)
            {
                case PumperState.GoingUp:
                case PumperState.OnTop:
                    _animator.Play("Armature_Up");
                    GoDown();
                    break;
            }
            if(_currentState == PumperState.RestPosition)
            {
                _animator.Play("Armature_Idle");
                AttackSides(cols);
            }
            return;
        }
        else
        {
            _animator.Play("Armature_Idle");
        }

        cols = CastPlat();
        if (cols.Length > 0)
        {
            switch (_currentState)
            {
                case PumperState.GoingDown:
                case PumperState.RestPosition:
                    _animator.Play("Armature_Down");
                    GoUp();
                    break;
            }
            return;
        }
        else
        {
            switch (_currentState)
            {
                case PumperState.GoingUp:
                case PumperState.OnTop:
                    _animator.Play("Armature_Up");
                    GoDown();
                    break;
            }
            return;
        }
    }

    void AttackSides(Collider[] cols)
    {
        foreach (Collider col in cols)
        {
            _gasRobber.TryToStealGas(col.transform.GetInstanceID());
        }
    }

    void GoDown()
    {
        if (_currentTween != null)
        {
            LeanTween.cancel(_currentTween.id);
            _currentTween = null;
        }

        float totalDistance = Vector3.Distance(_restPosition.transform.position, _upperPosition.transform.position);
        float currentDistance = Vector3.Distance(_pumper.transform.position, _restPosition.transform.position);

        float normalizedPosition = currentDistance / totalDistance;

        _currentTween = LeanTween
            .move(_pumper.gameObject, _restPosition, normalizedPosition * _timeToComplete)
            .setEase(_easetypeGoDown)
            .setOnComplete(() =>
            {
                _currentState = PumperState.RestPosition;
            });

        _currentState = PumperState.GoingDown;
    }

    void GoUp()
    {
        if (_currentTween != null)
        {
            LeanTween.cancel(_currentTween.id);
            _currentTween = null;
        }

        float totalDistance = Vector3.Distance(_restPosition.transform.position, _upperPosition.transform.position);
        float currentDistance = Vector3.Distance(_pumper.transform.position, _upperPosition.transform.position);

        float normalizedPosition = currentDistance / totalDistance;

        _currentTween = LeanTween
            .move(_pumper.gameObject, _upperPosition, normalizedPosition * _timeToComplete)
            .setEase(_easetypeGoUp)
            .setOnComplete(() =>
            {
                _currentState = PumperState.OnTop;
            });

        _currentState = PumperState.GoingUp;
    }

    Collider[] CastSides()
    {
        List<Collider> colls = new List<Collider>();
        colls.AddRange(Physics.OverlapBox(transform.position + _offsetFromCenter, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas));

        return colls.ToArray();
    }

    Collider[] CastPlat()
    {
        List<Collider> colls = new List<Collider>();
        colls.AddRange(Physics.OverlapBox(_pumper.transform.position + _scanBoxPlatOffest, _scanBoxPlatOffest, Quaternion.identity, _whereIsGas));

        return colls.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _scanBoxDebbugColor;

        Gizmos.DrawWireCube(transform.position + _offsetFromCenter, _scanBoxHalfExtends * 2);
        Gizmos.DrawWireCube(_pumper.transform.position + _scanBoxPlatOffest, _scanBoxPlatHalfExtends * 2);
    }
}
