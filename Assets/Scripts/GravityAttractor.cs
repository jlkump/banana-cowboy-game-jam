using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{

    public float gravity = -10.0f;

    public void Attract(Transform body, float max_fall_speed, float gravity_mult)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        //Vector3 bodyUp = body.up;

        //body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
        Vector3 fall_vec = body.InverseTransformDirection(body.GetComponent<Rigidbody>().GetRelativePointVelocity(Vector3.zero));
        fall_vec.x = 0;
        fall_vec.z = 0;

        if (fall_vec.magnitude < max_fall_speed)
        {
            if (fall_vec.y < 0)
            {
                // Here we are falling down, so we increase the strength of gravity slightly to make the fall faster
                body.GetComponent<Rigidbody>().AddForce(targetDir * gravity * gravity_mult * 1.5f);
            } 
            else
            {
                body.GetComponent<Rigidbody>().AddForce(targetDir * gravity * gravity_mult);
            }
        }
    }

    public void Reorient(Transform body)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
    }
}
