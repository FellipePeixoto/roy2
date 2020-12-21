using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;

    private void Reset()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void HideTrash()
    {
        gameObject.SetActive(false);
        _rb.velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void DropTrash(Vector3 newPosition)
    {
        gameObject.SetActive(true);
        _rb.velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = newPosition;
    }

    public void DestroyTrash()
    {
        Destroy(gameObject);
    }

    public Collider TrashCollider()
    {
        var a = GetComponent<Collider>();
        return a;
    }
}
