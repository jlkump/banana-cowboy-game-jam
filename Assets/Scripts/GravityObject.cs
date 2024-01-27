using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    List<GravityAttractor> attractors;
    bool on_ground = false;

    public LayerMask ground_mask;
    public float ground_detection_height = 10.0f;

    public float max_fall_speed { get; set; } = 30.0f;
    public float gravity_mult { get; set; } = 1.0f;

    void Awake()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        attractors = new List<GravityAttractor>();
    }

    void FixedUpdate()
    {
        int highest_prio_ind = GetHighestPrioAttractorIndex();
        if (highest_prio_ind != -1)
        {
            GravityAttractor attractor = attractors[highest_prio_ind];
            attractor.Reorient(transform);
            if (!on_ground)
            {
                attractor.Attract(transform, max_fall_speed, gravity_mult);
            }
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
            //print("Entered orbit");
            attractors.Add(collision.gameObject.GetComponent<GravityAttractor>());
        }
    }

    void OnTriggerExit(UnityEngine.Collider collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.GetComponent<GravityAttractor>() != null)
        {
            //print("Exited orbit");
            attractors.Remove(collision.gameObject.GetComponent<GravityAttractor>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject != null && 
            (ground_mask & 1 << collision.gameObject.layer) > 0) // Checking layer masks sucks -_-
            // Vector3.Dot((transform.position - collision.gameObject.transform.position, transform.up) > -0.3)
            // The above is used to check if the player is above the ground, but tends to be buggy since the player's up doesn't immediately change
        {
            print("On surface");
            on_ground = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision != null &&
            (ground_mask & 1 << collision.gameObject.layer) > 0)
            // Vector3.Dot((transform.position - collision.gameObject.transform.position, transform.up) > -0.3)
            // Same stuff
        {
            print("Left surface");
            on_ground = false;
        }
    }

    public bool IsOnGround()
    {
        return on_ground;
    }
}
