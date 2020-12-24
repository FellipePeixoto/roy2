using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : MonoBehaviour
{
    [Header("Configurações da zona magnetica")]
    [SerializeField] Vector3 p0;
    [SerializeField] Vector3 p1;
    [SerializeField] float radius;
    [SerializeField] LayerMask _whereIsMagnetics = (1 << 0);

    [SerializeField] float _speed;
    [SerializeField] float _minimumDistance;
    [SerializeField] Transform _start;
    [SerializeField] Transform _end;
    [SerializeField] bool stop;

    float _dir;
    Magnetic mag = null;

    Rigidbody _rb;
    Magnet _magnet;

    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _dir = 1;
    }

    private void FixedUpdate()
    {
        if (stop)
            return;

        var a = Physics.OverlapCapsule(p0, p1, radius, _whereIsMagnetics);
        if (a.Length > 0)
        {
            foreach (Collider col in a)
            {
                _magnet.TryAttractMagnetic(col.transform.GetInstanceID());
            }
        }

        Vector3 startPosi = new Vector3(_start.position.x, 0, 0);
        Vector3 endPosi = new Vector3(_end.position.x, 0, 0);
        Vector3 myPosi = new Vector3(_rb.position.x, 0, 0);
        float distanceStart = Vector3.Distance(myPosi, startPosi);
        float distanceEnd = Vector3.Distance(myPosi, endPosi);
        float dotStart = Vector3.Dot(Vector3.right * _dir, Vector3.left);
        float dotEnd = Vector3.Dot(Vector3.right * _dir, Vector3.right);

        if (_dir == 1 && (distanceEnd <= _minimumDistance || dotEnd < 0))
        {
            _dir = -1;
        }
        else if (_dir == -1 && (distanceStart <= _minimumDistance || dotStart < 0))
        {
            _dir = 1;
        }

        _rb.MovePosition(_rb.position + ((Vector3.right * _dir) * _speed * Time.fixedDeltaTime));
    }
}
