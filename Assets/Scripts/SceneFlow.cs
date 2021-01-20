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
    [Space]

    [Header("UI - Hidder")]
    [SerializeField] CanvasGroup _hidder;
    [SerializeField] float _fadeInTime;
    [SerializeField] float _fadeOutTime;
    [SerializeField] LeanTweenType _fadeOutType;
    [SerializeField] LeanTweenType _fadeInType;

    Roy _roy;
    Klunk _klunk;
    bool _changing;
    bool _resetingPosition;
    int _lastScene = 4;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();
    }

    private void FixedUpdate()
    {
        if (_changing || _resetingPosition)
            return;

        if (_roy == null || _klunk == null || _finishZone == null)
            return;

        if (!_bounds.Contains(_roy.transform.position) || !_bounds.Contains(_klunk.transform.position))
        {
            StartCoroutine(ReloadLoadSceneSmoth());
        }

        if (_finishZone.Contains(_roy.transform.position) && _finishZone.Contains(_klunk.transform.position))
        {
            StartCoroutine(LoadNextSceneSmoth());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(_finishZone.center, _finishZone.size);
    }

    public IEnumerator LoadNextSceneSmoth()
    {
        _changing = true;
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
        if (SceneManager.GetActiveScene().buildIndex + 1 <= _lastScene)
        {
            yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(0);
        }
        LeanTween.value(gameObject, (x) => { _hidder.alpha = x; }, 1, 0, _fadeOutTime)
                    .setEase(_fadeOutType);
        SceneFlow currentSceneFlow = FindObjectOfType<SceneFlow>();
        if (currentSceneFlow != null)
        {
            Destroy(gameObject);
            yield break;
        }
        _bounds = currentSceneFlow._bounds;
        _finishZone = currentSceneFlow._finishZone;
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();
        Destroy(currentSceneFlow.gameObject);
    }

    public IEnumerator ReloadLoadSceneSmoth()
    {
        _resetingPosition = true;
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
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        LeanTween.value(gameObject, (x) => { _hidder.alpha = x; }, 1, 0, _fadeOutTime)
                    .setEase(_fadeOutType);
        _resetingPosition = false;
        SceneFlow currentSceneFlow = FindObjectOfType<SceneFlow>();
        if (currentSceneFlow == null)
        {
            Destroy(gameObject);
            yield break;
        }
        _bounds = currentSceneFlow._bounds;
        _finishZone = currentSceneFlow._finishZone;
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();
        Destroy(currentSceneFlow.gameObject);
    }
}
