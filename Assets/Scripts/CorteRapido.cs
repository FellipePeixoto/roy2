using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorteRapido : MonoBehaviour
{
    [SerializeField] Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);
    [SerializeField] float _finish = 150;
    [SerializeField] Transform _royOrigin;
    [SerializeField] Transform _klunkOrigin;
    [SerializeField] int _wichOne = -10;

    Roy _roy;
    Klunk _klunk;

    bool _changing;

    private void Awake()
    {
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();
    }

    private void FixedUpdate()
    {
        if (_changing)
            return;

        if (!_bounds.Contains(_roy.transform.position) || !_bounds.Contains(_klunk.transform.position))
        {
            _roy.transform.position = _royOrigin.transform.position;
            _klunk.transform.position = _klunkOrigin.transform.position;
            _roy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _klunk.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _roy.GetComponent<Rigidbody>().isKinematic = true;
            _klunk.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(Wait2Frame());
        }

        if (_roy.transform.position.x > _finish && _klunk.transform.position.x > _finish)
        {
            SceneSingleton.LoadScene(_wichOne);
            _changing = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0,0.7f,1,0.5f);
        Gizmos.DrawCube(_bounds.center, _bounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(_finish, -100), new Vector3(_finish,100));
    }

    IEnumerator Wait2Frame()
    {
        yield return new WaitForSeconds(.75f);
        _roy.GetComponent<Rigidbody>().isKinematic = false;
        _klunk.GetComponent<Rigidbody>().isKinematic = false;
    }
}
