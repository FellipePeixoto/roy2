using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [SerializeField] Magnet _magnet;
    [Space]

    [Header("Configurações da zona magnetica")]
    [SerializeField] Vector3 _halfExtends = new Vector3(8, 1.8f, 2);
    [SerializeField] Color _debbugMagnetZoneColor = Color.white;
    [SerializeField] LayerMask _whereIsMagnetics = (1 << 0);

    [SerializeField] Animator _animator;

    private void Reset()
    {
        _magnet = GetComponent<Magnet>();
    }

    private void Awake()
    {
        _animator.Play("Armação_Walk");
    }

    private void FixedUpdate()
    {
        Collider[] inSide = Physics.OverlapBox(_mainCollider.bounds.center, _halfExtends, Quaternion.identity, _whereIsMagnetics);
        if (inSide.Length > 0)
        {
            foreach (Collider col in inSide)
            {
                _magnet.TryAttractMagnetic(col.transform.GetInstanceID());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 extends = _halfExtends * 2;

        Gizmos.color = _debbugMagnetZoneColor;
        Gizmos.DrawWireCube(_mainCollider.bounds.center, extends);
    }
}
