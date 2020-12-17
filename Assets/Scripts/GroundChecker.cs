using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] LayerMask whatIsGround;

    bool isGrounded;
    string _other;
    List<Collider> _colliders = new List<Collider>();

    public bool IsGrounded()
    {
        return isGrounded;
    }

    private void OnTriggerStay(Collider other)
    {
        if (1 << other.gameObject.layer == whatIsGround && !_colliders.Contains(other))
        {
            _colliders.Add(other);
            _other = other.name;
            isGrounded = true;
            var disBCol = other.GetComponent<DisabledCollider>();
            if (disBCol)
            {
                disBCol.OnDisableCollider += DisBCol_OnDisableCollider;
            }
        }
    }

    private void DisBCol_OnDisableCollider(Collider other)
    {
        _colliders.Remove(other);
        isGrounded = false;
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
        isGrounded = false;
    }
}
