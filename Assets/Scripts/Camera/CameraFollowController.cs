using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] Vector3 _offset;

    Rigidbody rb;

    private void Start()
    {
        rb = _target.GetComponent<Rigidbody>();

        if (!_target)
            return;

        transform.position = Vector3.Lerp
            (transform.position,
            new Vector3(_target.transform.position.x,
            _target.transform.position.y + _offset.y,
            transform.position.z + _offset.z), 15 * Time.smoothDeltaTime);
    }

    private void FixedUpdate()
    {
        if (!_target)
            return;

        transform.position = Vector3.Lerp
            (transform.position, 
            new Vector3(_target.transform.position.x,
            _target.transform.position.y + _offset.y,
            transform.position.z + _offset.z), 15 * Time.smoothDeltaTime);
    }

    public void SetPlayer()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
    }
}


