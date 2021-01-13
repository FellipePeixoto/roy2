using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PumperState { RestPosition, GoingUp, OnTop, GoingDown }

public class Pumper : MonoBehaviour
{
    [SerializeField] Transform _pumper;
    [SerializeField] Transform _restPosition;
    [SerializeField] Transform _upperPosition;
    [SerializeField] LeanTweenType _easetype;
    [SerializeField] float _timeToComplete;
    [SerializeField] Vector3 _scanBoxPlatOffest;
    [SerializeField] Vector3 _scanBoxPlatHalfExtends;
    [SerializeField] Vector3 _scanBoxHalfExtends = new Vector3(.5f, 1, .5f);
    [SerializeField] float _offsetFromCenter = 3;
    [SerializeField] Color _scanBoxDebbugColor = Color.red;
    [SerializeField] LayerMask _whereIsGas = (1 << 0);
    [SerializeField] GasRobber _gasRobber;

    PumperState _currentState = PumperState.RestPosition;
    LTDescr _goUp;

    private void FixedUpdate()
    {
        Collider[] cols = CastSides();
        if (cols.Length > 0)
        {
            AttackSides();
            return;
        }

        cols = CastPlat();
        if (cols.Length > 0)
        {
            GoingUp();
        }

        if (_currentState == PumperState.OnTop)
        {
            GoDown();
        }
    }

    void AttackSides()
    {
        switch (_currentState)
        {
            case PumperState.GoingUp:
            case PumperState.OnTop:
                if (_goUp != null)
                    GoDown();
                break;

            case PumperState.RestPosition:
                Collider[] cols = CastSides();
                foreach (Collider col in cols)
                {
                    _gasRobber.TryToStealGas(col.transform.GetInstanceID());
                }
                break;
        }
    }

    void GoDown()
    {
        if (_goUp != null)
        {
            LeanTween.cancel(_goUp.id);
            _goUp = null;
        }

        _currentState = PumperState.GoingDown;

        LeanTween
            .move(_pumper.gameObject, _restPosition, _timeToComplete)
            .setEase(_easetype)
            .setOnComplete(() =>
            {
                _currentState = PumperState.RestPosition;
            });
    }

    void GoingUp()
    {
        _currentState = PumperState.GoingUp;
        if (_goUp == null)
        {
            _goUp = LeanTween
            .move(_pumper.gameObject, _upperPosition, _timeToComplete)
            .setEase(_easetype)
            .setOnComplete(() =>
            {
                _currentState = PumperState.OnTop;
            });
        }
    }

    Collider[] CastSides()
    {
        List<Collider> colls = new List<Collider>();
        colls.AddRange(Physics.OverlapBox(transform.position + Vector3.right * _offsetFromCenter, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas));
        colls.AddRange(Physics.OverlapBox(transform.position + Vector3.left * _offsetFromCenter, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas));

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

        Gizmos.DrawWireCube(transform.position + Vector3.right * _offsetFromCenter, _scanBoxHalfExtends * 2);
        Gizmos.DrawWireCube(transform.position + Vector3.left * _offsetFromCenter, _scanBoxHalfExtends * 2);
        Gizmos.DrawWireCube(_pumper.transform.position + _scanBoxPlatOffest, _scanBoxPlatHalfExtends * 2);
    }
}
