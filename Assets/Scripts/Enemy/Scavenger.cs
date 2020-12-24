using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [SerializeField] Rigidbody _rb;
    [SerializeField] Magnet _magnet;

    [Header("Configurações da zona magnetica")]
    [SerializeField] Vector3 _halfExtends = new Vector3(8, 1.8f, 2);
    [SerializeField] Color _debbugMagnetZoneColor = Color.white;
    [SerializeField] LayerMask _whereIsMagnetics = (1 << 0);

    [SerializeField] float _speed;
    [SerializeField] float _minimumDistance;
    [SerializeField] Transform _start;
    [SerializeField] Transform _end;
    [SerializeField] bool stop;

    float _dir;
    Magnetic mag = null;


    private void Reset()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _mainCollider = GetComponent<Collider>();
        _magnet = GetComponent<Magnet>();
    }

    private void FixedUpdate()
    {
        if (stop)
            return;

        Collider[] inSide = Physics.OverlapBox(_mainCollider.bounds.center, _halfExtends, Quaternion.identity, _whereIsMagnetics);
        if (inSide.Length > 0)
        {
            foreach (Collider col in inSide)
            {
                _magnet.TryAttractMagnetic(col.transform.GetInstanceID());
            }
        }

        //Vector3 startPosi = new Vector3(_start.position.x, 0, 0);
        //Vector3 endPosi = new Vector3(_end.position.x, 0, 0);
        //Vector3 myPosi = new Vector3(_rb.position.x, 0, 0);
        //float distanceStart = Vector3.Distance(myPosi, startPosi);
        //float distanceEnd = Vector3.Distance(myPosi, endPosi);
        //float dotStart = Vector3.Dot(Vector3.right * _dir, Vector3.left);
        //float dotEnd = Vector3.Dot(Vector3.right * _dir, Vector3.right);

        //if (_dir == 1 && (distanceEnd <= _minimumDistance || dotEnd < 0))
        //{
        //    _dir = -1;
        //}
        //else if (_dir == -1 && (distanceStart <= _minimumDistance || dotStart < 0))
        //{
        //    _dir = 1;
        //}

        //_rb.MovePosition(_rb.position + ((Vector3.right * _dir) * _speed * Time.fixedDeltaTime));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _debbugMagnetZoneColor;
        Gizmos.DrawWireCube(_mainCollider.bounds.center, _halfExtends * 2);
    }
}
