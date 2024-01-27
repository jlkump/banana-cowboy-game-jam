using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{

    public float gravity = -10.0f;
    public int priority = 0; // Higher priority overrides lower priority
    public enum AttractDirection
    {
        RADIAL,
        OBJECT_X,
        OBJECT_Y,
        OBJECT_Z
    }

    public AttractDirection attract_direction = AttractDirection.RADIAL;
    public Transform center_of_gravity; // Only really matters for radial

    public void Awake()
    {
        center_of_gravity = transform;
    }

    public void Attract(Transform body, float max_fall_speed, float gravity_mult)
    {
        Vector3 targetDir = GetGravityUp(body);
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

    public int GetPriority()
    {
        return priority;
    }

    public void Reorient(Transform body)
    {
        Vector3 targetDir = GetGravityUp(body);
        Vector3 bodyUp = body.up;
        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
    }

    Vector3 GetGravityUp(Transform body)
    {
        switch (attract_direction)
        {
            case AttractDirection.OBJECT_X:
                return transform.right;
            case AttractDirection.OBJECT_Y:
                return transform.up;
            case AttractDirection.OBJECT_Z:
                return transform.forward;
            case AttractDirection.RADIAL:
            default:
                return (body.position - center_of_gravity.position);
        }
    }
}
