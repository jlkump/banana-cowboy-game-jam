using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class GravityObject : MonoBehaviour
{
    GravityAttractor planet;
    bool on_ground = false;

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
            planet.Attract(transform);
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        print("Collision enter with " + collision.gameObject.name);
        if (planet != null && collision.gameObject.tag == planet.tag)
        {
            print("Is on ground");
            on_ground = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        print("Collision exit with " + collision.gameObject.name);
        if (planet != null && collision.gameObject.tag == planet.tag)
        {
            print("Is not on ground");
            on_ground = false;
        }
    }

    public bool IsOnGround()
    {
        return on_ground;
    }
}
