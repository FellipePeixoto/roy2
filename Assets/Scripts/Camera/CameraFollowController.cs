using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] Vector3 _offset;
    
    private void FixedUpdate()
    {
        if (!_target)
            return;

        var x = 1;

        if (!_target.GetComponent<RoyMovementPattern>()._facedRight)
            x = -1;

        transform.position = Vector3.Lerp
            (transform.position, 
            new Vector3(_target.transform.position.x + (_offset.x * x),
            _target.transform.position.y + _offset.y,
            transform.position.z + _offset.z), 10 * Time.smoothDeltaTime);
    }

    public void SetPlayer()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
    }
}


