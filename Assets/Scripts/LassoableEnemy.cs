using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LassoableEnemy : MonoBehaviour
{
    Transform lasso_actor_position;

    public float orbit_radius = 1.0f;
    public float orbit_radius_speed = 1.0f;
    public float orbit_radius_oscillation = 0.0f;
    public float orbit_radius_oscillation_speed = 1.0f;

    public float orbit_height_offset = 1.0f;
    public float orbit_height_oscillation = 0.5f;
    public float orbit_height_oscillation_speed = 1.0f;

    private float orbit_time = 0.0f;

    private bool thrown = false;

    // Update is called once per frame
    void Update()
    {
        if (lasso_actor_position != null)
        {
            Vector3 up = lasso_actor_position.up;
            Vector3 right = lasso_actor_position.right;
            Vector3 forward = lasso_actor_position.forward;

            orbit_time += Time.deltaTime;

            float x = orbit_radius * Mathf.Cos(orbit_time * orbit_radius_speed);
            float y = orbit_height_offset + orbit_height_oscillation * Mathf.Cos(orbit_time * orbit_height_oscillation_speed);
            float z = orbit_radius * Mathf.Sin(orbit_time * orbit_radius_speed);

            transform.position = lasso_actor_position.position + up * y + right * x + forward * z;
        }
    }

    public void SetLassoActor(Transform lasso_actor)
    {
        lasso_actor_position = lasso_actor;
    }

    public void ThrowEnemyInDirection(Vector3 dir, float force)
    {
        thrown = true;
        lasso_actor_position = null;
        GetComponent<Rigidbody>().velocity = dir * force;
        //GetComponent<Rigidbody>().AddForce(transform.up * force, ForceMode.Impulse);

    }

    public void ThrowEnemyAtTarget(Transform target)
    {
        thrown = true;
        // TODO:
        // 1. Add state to lassoable enemy to move towards target in an arc
        // 2. When hits target, explodes into juice
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Contact!");
        if (thrown)
        {
            // SPLAT!
            Destroy(gameObject);
        }
    }
}
