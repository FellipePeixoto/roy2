using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EletricThread : MonoBehaviour
{
    [SerializeField] Bounds _killZone;
    [SerializeField] Animator _animator;
    [SerializeField] LayerMask _whereIsPlayers;

    private void Awake()
    {
        _animator.Play("Armação_Action");
    }

    bool _end;

    private void FixedUpdate()
    {
        Collider[] cols =  
            Physics.OverlapBox(transform.position, _killZone.extents, Quaternion.identity, _whereIsPlayers);

        if (cols.Length > 0)
        {
            SceneFlow sceneFlow = FindObjectOfType<SceneFlow>();
            if (sceneFlow != null)
            {
                _end = false;
                sceneFlow.StartCoroutine(sceneFlow.ReloadLoadSceneSmoth());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _killZone.size);
    }
}
