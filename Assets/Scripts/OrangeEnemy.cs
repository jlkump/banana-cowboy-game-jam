using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OrangeEnemy : MonoBehaviour
{

    public UIManager UI;

    private enum State
    {
        IDLE,
        CHASING,
        RECHARGE,
    }
    private State state = State.IDLE;

    public float accel_rate = 0.2f;
    public float starting_speed = 0.2f;
    public float max_speed = 1.0f;
    private float current_speed = 0.2f;

    public float attack_recharge_time = 1.0f;
    private float attack_recharge_timer = 0.0f;

    private GameObject target = null;

    void Awake()
    {
        UI = GameObject.Find("Player UI").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.IDLE:
                break;
            case State.RECHARGE:
                attack_recharge_timer += Time.deltaTime;
                if (attack_recharge_timer >= attack_recharge_time)
                {
                    attack_recharge_timer = 0.0f;
                    if (target != null)
                    {
                        state = State.CHASING;
                    } 
                    else
                    {

                        state = State.IDLE;
                    }
                }
                break;
            case State.CHASING:
                if (target != null)
                {
                    if (GetComponent<Rigidbody>().velocity.magnitude < max_speed) {
                        //print("Adding velocity b/c magnitude is " + GetComponent<Rigidbody>().velocity.magnitude + " and max speed " + max_speed);
                        current_speed += accel_rate;
                    }
                    GetComponent<Rigidbody>().velocity = (target.transform.position - transform.position).normalized * current_speed;
                    transform.rotation = Quaternion.LookRotation((target.transform.position - transform.position).normalized, transform.up);
                    //print("Moving with vel " + GetComponent<Rigidbody>().velocity + " with magnitude " + GetComponent<Rigidbody>().velocity.magnitude);
                }
                break;
        }
        //print("State is " + state);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject != null && other.tag == "Player")
        {
            target = other.gameObject;
            state = State.CHASING;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject != null && other.tag == "Player")
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            target = null;
            state = State.IDLE;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.GetComponent<ThirdPersonController>() != null)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            UI.ChangeHealth(-1);
            state = State.RECHARGE;
            attack_recharge_timer = 0.0f;
            current_speed = starting_speed;
        }
    }
}
