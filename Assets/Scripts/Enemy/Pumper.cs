using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumper : MonoBehaviour
{
    [SerializeField] Vector3 _scanBoxHalfExtends = new Vector3(.5f, 1, .5f);
    [SerializeField] float _offsetFromCenter = 3;
    [SerializeField] Color _scanBoxDebbugColor = Color.red;

    private void FixedUpdate()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _scanBoxDebbugColor;

        Gizmos.DrawWireCube(transform.position + Vector3.right * _offsetFromCenter, _scanBoxHalfExtends * 2);
        Gizmos.DrawWireCube(transform.position + Vector3.left * _offsetFromCenter, _scanBoxHalfExtends * 2);
    }
}
