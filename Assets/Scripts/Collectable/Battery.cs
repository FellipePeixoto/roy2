using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] float boxCastSizeOffset = .9f;

    private void FixedUpdate()
    {
        Collider col = GetComponent<Collider>();
        RaycastHit hitInfo;
        bool hit = Physics.BoxCast(col.bounds.center,
            (col.bounds.size * boxCastSizeOffset) / 2,
            Vector3.down,
            out hitInfo,
            Quaternion.identity,
            100, 1 << LayerMask.NameToLayer("Ground"));
        if (hit)
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Roy possivleRoy = other.GetComponent<Roy>();

        if (!possivleRoy)
            return;

        //TODO: ADD BATTERY
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Collider col = GetComponent<Collider>();
        RaycastHit hitInfo;

        bool hit = Physics.BoxCast(col.bounds.center,
            (col.bounds.size * boxCastSizeOffset) / 2,
            -transform.up,
            out hitInfo,
            Quaternion.identity,
            100, 1 << LayerMask.NameToLayer("Ground"));

        //Check if there has been a hit yet
        if (hit)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(col.bounds.center, -transform.up * hitInfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(col.bounds.center + -transform.up * hitInfo.distance, col.bounds.size);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(col.bounds.center, -transform.up * 100);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(col.bounds.center + -transform.up * 100, col.bounds.size);
        }
    }
}
