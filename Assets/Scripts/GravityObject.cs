using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    List<GravityAttractor> attractors;
    bool on_ground = false;

    public float max_fall_speed { get; set; } = 40.0f;
    public float gravity_mult { get; set; } = 1.5f;

    void Awake()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        int highest_prio_ind = GetHighestPrioAttractorIndex();
        if (highest_prio_ind != -1)
        {
            GravityAttractor attractor = attractors[highest_prio_ind];
            attractor.Attract(transform, max_fall_speed, gravity_mult);
        }
    }

    int GetHighestPrioAttractorIndex()
    {
        int index = -1;
        int highest_prio = int.MinValue;
        for (int i = 0; i < attractors.Count; i++)
        {
            if (attractors[i].GetPriority() > highest_prio)
            {
                highest_prio = attractors[i].GetPriority();
                index = i;
            }
        }

        return index;
    }

    void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.GetComponent<GravityAttractor>() != null)
        {
            attractors.Add(collision.gameObject.GetComponent<GravityAttractor>());
        }
    }

    void OnTriggerExit(UnityEngine.Collider collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.GetComponent<GravityAttractor>() != null)
        {
            attractors.Remove(collision.gameObject.GetComponent<GravityAttractor>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.tag == "ground")
        {
            on_ground = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.tag == "ground")
        {
            on_ground = false;
        }
    }

    public bool IsOnGround()
    {
        return on_ground;
    }
}
