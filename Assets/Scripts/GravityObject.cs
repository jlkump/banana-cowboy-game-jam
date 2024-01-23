using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    GravityAttractor planet;
    bool on_ground = false;

    public float max_fall_speed { get; set; } = 40.0f;
    public float gravity_mult { get; set; } = 1.0f;

    void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

    }

    void FixedUpdate()
    {
        planet.Reorient(transform);
        if (planet != null && !on_ground)
        {
            planet.Attract(transform, max_fall_speed, gravity_mult);
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (planet != null && collision.gameObject.tag == planet.tag)
        {
            on_ground = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (planet != null && collision.gameObject.tag == planet.tag)
        {
            on_ground = false;
        }
    }

    public bool IsOnGround()
    {
        return on_ground;
    }
}
