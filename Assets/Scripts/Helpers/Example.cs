//Attach this script to a GameObject. Make sure it has a Collider component by clicking the Add Component button. Then click Physics>Box Collider to attach a Box Collider component.
//This script creates a BoxCast in front of the GameObject and outputs a message if another Collider is hit with the Collider’s name.
//It also draws where the ray and BoxCast extends to. Just press the Gizmos button to see it in Play Mode.
//Make sure to have another GameObject with a Collider component for the BoxCast to collide with.

using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField] float m_MaxDistance;
    [SerializeField] Collider m_Collider;
    [SerializeField] LayerMask whatIsGround;
    [Range(0, 1)] [SerializeField] float percent = 1;
    bool m_HitDetect;

    RaycastHit m_Hit;

    void Update()
    {
        //Simple movement in x and z axes
        //float xAxis = Input.GetAxis("Horizontal") * m_Speed;
        //float zAxis = Input.GetAxis("Vertical") * m_Speed;
        //transform.Translate(new Vector3(xAxis, 0, zAxis));
    }

    void FixedUpdate()
    {
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data
        m_HitDetect = Physics.BoxCast(m_Collider.bounds.center, (m_Collider.bounds.size * percent) / 2, transform.right, out m_Hit, transform.rotation, m_MaxDistance, whatIsGround);
        if (m_HitDetect)
        {
            //Output the name of the Collider your Box hit
            Debug.Log("Hit : " + m_Hit.collider.name);
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (m_HitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(m_Collider.bounds.center, transform.right * m_Hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(m_Collider.bounds.center + transform.right * m_Hit.distance, m_Collider.bounds.size * percent);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(m_Collider.bounds.center, transform.right * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(m_Collider.bounds.center + transform.right * m_MaxDistance, m_Collider.bounds.size * percent);
        }
    }
}