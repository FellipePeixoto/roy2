using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PumperState { Idle, OnTheWay, GetBack, Attacking }

public class Pumper : MonoBehaviour
{
    [SerializeField] Transform _pumper;
    [SerializeField] Transform _restPosition;
    [SerializeField] Transform _lowPosition;
    [SerializeField] LeanTweenType _easetype;
    [SerializeField] float _timeToComplete;
    [SerializeField] Vector3 _scanBoxHalfExtends = new Vector3(.5f, 1, .5f);
    [SerializeField] float _offsetFromCenter = 3;
    [SerializeField] Color _scanBoxDebbugColor = Color.red;
    [SerializeField] LayerMask _whereIsGas = (1 << 0);
    [SerializeField] GasRobber _gasRobber;

    bool _busy;
    PumperState _currentState = PumperState.Idle;

    private void FixedUpdate()
    {
        if (_busy)
        {

            return;
        }

        Collider[] colls = Physics.OverlapBox(transform.position, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas);
        if (colls.Length > 0)
        {
            StartCoroutine(Trigger());
            StartCoroutine(TrySteal());
            StartCoroutine(BackToNormal());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _scanBoxDebbugColor;

        Gizmos.DrawWireCube(transform.position + Vector3.right * _offsetFromCenter, _scanBoxHalfExtends * 2);
        Gizmos.DrawWireCube(transform.position + Vector3.left * _offsetFromCenter, _scanBoxHalfExtends * 2);
    }

    IEnumerator Trigger() 
    {
        _busy = true;
        _currentState = PumperState.OnTheWay;
        LeanTween
            .move(_pumper.gameObject, _lowPosition, _timeToComplete)
            .setEase(_easetype);
        yield return new WaitUntil(() => { return _pumper.transform.position.y == 0; });
        Collider[] colls = Physics.OverlapBox(transform.position, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas);
        if (colls.Length > 0)
        {
            foreach(Collider col in colls)
            {
                _gasRobber.TryToStealGas(col.transform.GetInstanceID());
            }
            _currentState = PumperState.Attacking;
        }
        else
        {
            _currentState = PumperState.GetBack;
        }
    }

    IEnumerator TrySteal()
    {
        yield return new WaitUntil(() => { return _currentState == PumperState.Attacking; });
        Collider[] colls = Physics.OverlapBox(transform.position, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas);
        while (colls.Length > 0)
        {
            foreach (Collider col in colls)
            {
                _gasRobber.TryToStealGas(col.transform.GetInstanceID());
            }
            colls = Physics.OverlapBox(transform.position, _scanBoxHalfExtends, Quaternion.identity, _whereIsGas);
            yield return new WaitForFixedUpdate();
        }
        _currentState = PumperState.GetBack;
    }

    IEnumerator BackToNormal()
    {
        yield return new WaitUntil(() => { return _currentState == PumperState.GetBack; });
        LeanTween
            .move(_pumper.gameObject, _restPosition, _timeToComplete)
            .setEase(_easetype)
            .setOnComplete(()=> { _busy = false; });
    }
}
