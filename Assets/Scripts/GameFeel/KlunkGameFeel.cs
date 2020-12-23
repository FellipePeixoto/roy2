using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KlunkGameFeel : MonoBehaviour
{
    [Header("SKATE")]
    [SerializeField] GameObject _skateObject;
    [Space]

    [Header("INVESTIDA")]
    [SerializeField] GameObject _shieldObject;
    [SerializeField] GameObject _trailPrefab;
    [SerializeField] GameObject[] _dashTrailPoints;
    [Tooltip("EM SEGUNDOS")]
    [SerializeField] float timerToDestroyTrails = 2;

    [Space]
    [SerializeField] GameObject _boxPickUp;

    Klunk _klunk;
    KlunkInteractController _klunkInteractController;
    GameObject[] _trailsObjects;
    bool _dashing;

    private void Reset()
    {
        _trailPrefab = Resources.Load<GameObject>("Prefabs/Set_shielddash_trail");
    }

    private void Awake()
    {
        _klunk = GetComponent<Klunk>();
        _klunkInteractController = GetComponent<KlunkInteractController>();

        _klunk.OnStartDash += _klunk_OnStartDash;
        _klunk.OnEndDash += _klunk_OnEndDash;
        _klunk.OnStartSkate += _klunk_OnStartSkate;
        _klunk.OnEndSkate += _klunk_OnEndSkate;
        _klunkInteractController.OnPickStart += _klunkInteractController_OnPickStart;
        _klunkInteractController.OnDropStart += _klunkInteractController_OnPDropStart;
    }

    private void _klunkInteractController_OnPDropStart(float time)
    {
        if (_boxPickUp != null)
        {
            _boxPickUp.SetActive(true);
            LeanTween.scale(_boxPickUp, Vector3.one, time)
                .setEaseInExpo()
                .setOnComplete(() => { _boxPickUp.SetActive(false);  });
        }
    }

    private void _klunkInteractController_OnPickStart(float time)
    {
        if (_boxPickUp != null)
        {
            _boxPickUp.SetActive(true);
            LeanTween.scale(_boxPickUp, Vector3.one * .1f, time)
                .setEaseOutExpo()
                .setOnComplete(() => { _boxPickUp.SetActive(false); });
        }
    }

    private void _klunk_OnStartSkate(Vector3 startPoint)
    {
        if (_skateObject != null) _skateObject.SetActive(true);
    }

    private void _klunk_OnEndSkate(Vector3 startPoint)
    {
        if (_skateObject != null) _skateObject.SetActive(false);
    }

    private void _klunk_OnStartDash(Vector3 startPoint)
    {
        _dashing = true;
        if (_shieldObject != null) _shieldObject.SetActive(true);
        _trailsObjects = new GameObject[_dashTrailPoints.Length];
        for (int i = 0; i < _dashTrailPoints.Length; i++)
        {
            _trailsObjects[i] = Instantiate(_trailPrefab, 
                _dashTrailPoints[i].transform.position,
                Quaternion.identity) ;
        }
    }

    private void _klunk_OnEndDash(Vector3 startPoint)
    {
        _dashing = false;
        if (_shieldObject != null) _shieldObject.SetActive(false);
        foreach(GameObject g in _trailsObjects)
        {
            Destroy(g.gameObject, timerToDestroyTrails);
        }
    }
    private void LateUpdate()
    {
        if (!_dashing)
            return;

        for (int i = 0; i < _dashTrailPoints.Length; i++)
        {
            _trailsObjects[i].transform.position = 
                _dashTrailPoints[i].transform.position;
        }
    }

    private void OnDestroy()
    {
        _klunk.OnStartDash -= _klunk_OnStartDash;
        _klunk.OnEndDash -= _klunk_OnEndDash;
        _klunk.OnStartSkate -= _klunk_OnStartSkate;
        _klunk.OnEndSkate -= _klunk_OnEndSkate;
        _klunkInteractController.OnPickStart -= _klunkInteractController_OnPickStart;
        _klunkInteractController.OnDropStart -= _klunkInteractController_OnPDropStart;
    }
}
