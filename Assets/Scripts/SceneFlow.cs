using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFlow : MonoBehaviour
{
    [Header("Level Bounds and Finish")]
    [SerializeField] Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);
    [SerializeField] Bounds _finishZone = new Bounds(Vector3.zero, Vector3.one * 2);
    [SerializeField] int _wichOne = -10;
    public Bounds Bounds { get { return _bounds; } }
    public Bounds FinishZone { get { return _finishZone; } }
    public int WichOne { get { return _wichOne; } }
    [Space]

    [SerializeField] Transform _royOrigin;
    [SerializeField] Transform _klunkOrigin;
    [Space]

    [Header("UI - Respawn")]
    [SerializeField] CanvasGroup _hidder;
    [SerializeField] float _fadeInTime;
    [SerializeField] float _fadeOutTime;
    [SerializeField] LeanTweenType _fadeOutType;
    [SerializeField] LeanTweenType _fadeInType;

    Roy _roy;
    Klunk _klunk;
    bool _changing;
    bool _resetingPosition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();
        _royOrigin.transform.position = _roy.transform.position;
        _klunkOrigin.transform.position = _klunk.transform.position;
    }

    private void FixedUpdate()
    {
        if (_changing || _resetingPosition)
            return;

        if (!_roy || !_klunk)
            return;

        if (!_bounds.Contains(_roy.transform.position) || !_bounds.Contains(_klunk.transform.position))
        {
            _resetingPosition = true;
            StartCoroutine(ResetPositionSmoth());
        }

        if (_finishZone.Contains(_roy.transform.position) && _finishZone.Contains(_klunk.transform.position))
        {
            _changing = true;
            StartCoroutine(LoadNewSceneSmoth());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(_finishZone.center, _finishZone.size);
    }

    IEnumerator LoadNewSceneSmoth()
    {
        _roy = null;
        _klunk = null;
        bool ok = false;
        LeanTween.value(gameObject, (x) => { _hidder.alpha = x; }, 0, 1, _fadeInTime)
            .setEase(_fadeInType)
            .setOnComplete(() =>
            {
                ok = true;
            });
        yield return new WaitUntil(() => { return ok; });
        yield return SceneManager.LoadSceneAsync(_wichOne);
        SceneFlow currentSceneFlow = FindObjectOfType<SceneFlow>();
        if (currentSceneFlow == null)
        {
            Destroy(gameObject);
            yield break;
        }
        Destroy(currentSceneFlow);
        _bounds = currentSceneFlow._bounds;
        _finishZone = currentSceneFlow._finishZone;
        _wichOne = currentSceneFlow.WichOne;
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();
        _royOrigin.transform.position = _roy.transform.position;
        _klunkOrigin.transform.position = _klunk.transform.position;
    }

    IEnumerator ResetPositionSmoth()
    {
        LeanTween.value(gameObject, (x) => { _hidder.alpha = x; }, 0, 1, _fadeInTime)
            .setEase(_fadeInType)
            .setOnComplete(() =>
            {
                _roy.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _roy.GetComponent<Rigidbody>().MovePosition(_royOrigin.transform.position);
                _klunk.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _klunk.GetComponent<Rigidbody>().MovePosition(_klunkOrigin.transform.position);
                _resetingPosition = false;
                LeanTween.value(gameObject, (x) => { _hidder.alpha = x; }, 1, 0, _fadeOutTime)
                    .setEase(_fadeOutType);
            });
        yield return null;
    }
}
