using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour
{
    Transform cameraTransform;
    [Header("References")]
    public Transform player_root;
    public Transform modelTransform;
    public LineRenderer lr;
    public Transform lasso_throw_pos;
    //public LayerMask swingable;

    //public Transform temp;


    [Header("Movement")]
    public float max_walk_speed = 8.0f;
    public float max_run_speed = 12.0f;
    public float accel_rate = (50.0f * 0.5f) / 15.0f;
    public float decel_rate = (100.0f * 0.5f) / 15.0f;
    public float accel_in_air_rate = 0.8f;
    public float decel_in_air_rate = 0.8f;
    public float jump_impulse_force = 10.0f;
    public float gravity_mult_on_jump_release = 3.0f;
    public bool conserve_momentum = true;
    public float dash_force;
    private bool can_Dash = true;
    private float dash_Cooldown = 0.75f;
    private float dash_Timer = 0.0f;
    public float LastOnGroundTime { get; private set; }

    [Header("Buffer System")]
    public float jump_hold_buffer = 0.3f;
    private float jump_hold_buffer_timer;
    public float jump_buffer = 0.1f;
    private float jump_buffer_timer;

    [Header("Lasso")]
    public float lasso_reach_distance = 25f;
    public float lasso_dectection_distance = 50f;
    public GameObject lasso_scope_indicator;
    //public Material lasso_scope_mat;
    public int max_number_of_indicators = 10;

    public Color targeted_color = Color.green;
    public Color out_of_range_color = Color.red;
    public Color in_range_color = Color.grey;

    private GameObject lasso_target; // Null when no valid target. Targets are swingable, enemy, and collectable
    private List<GameObject> indicators;


    [Header("Swinging")]
    private Vector3 swing_point;
    private SpringJoint swing_joint;
    private Vector3 current_rope_end_pos;

    [Header("Input")]
    public KeyCode lassoKey = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    private Vector3 _moveInput;

    enum LassoState
    {
        NONE,
        WOUND_UP,
        SWING,
        ENEMY
    }
    private LassoState lasso_state = LassoState.NONE;

    enum State
    {
        AIR,
        WALK,
        RUN,
    };
    private State player_state = State.AIR; // TODO: Add methods for SetState, IsValidState for state machine

    void Start()
    {
        cameraTransform = Camera.main.transform;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        jump_hold_buffer_timer = 0.0f;
        jump_buffer_timer = 0.0f;

        indicators = new List<GameObject>(max_number_of_indicators);
        for (int i = 0; i < max_number_of_indicators; i++)
        {
            GameObject indicator = Instantiate(lasso_scope_indicator, transform.position, Quaternion.identity, GameObject.Find("Player UI").transform);
            indicator.SetActive(false); // Should make invisible, we shall see
            indicators.Add(indicator);
        }
        lasso_target = null;
    }

    void Update()
    {
        // TODO: Remove this, is just for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            UIManager.ChangeHealth(-1);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            UIManager.ChangeHealth(1);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(horizontal, 0, vertical).normalized;

        LastOnGroundTime -= Time.deltaTime;
        if (Input.GetKeyDown(jumpKey))
        {
            jump_buffer_timer = jump_buffer;
        }

        if (GetComponent<GravityObject>().IsOnGround())
        {
            LastOnGroundTime = 0.1f;
            player_state = Input.GetKeyDown(sprintKey) ? State.RUN : State.WALK;
            if (Input.GetKeyDown(jumpKey)) 
            {
                StartJump();
            }
            else if(jump_buffer_timer > 0)
            {
                StartJump();
            }
        }
        else
        {
            player_state = State.AIR;
        }

        if (Input.GetKeyDown(lassoKey))
        {
            switch(lasso_state)
            {
                case LassoState.NONE:
                    print("lassoing windup!");
                    StartLassoWindup();
                    break;
                case LassoState.WOUND_UP:
                case LassoState.SWING:
                case LassoState.ENEMY:
                default:
                    // Shouldn't be possible
                    print("Click down on invalid lasso state");
                    break;
                }
        }

        if (Input.GetKeyUp(lassoKey)) { 

            switch (lasso_state)
            {

                case LassoState.WOUND_UP:
                    print("lasso windup release");
                    EndLassoWindup();
                    break;
                case LassoState.SWING:
                    print("lasso end swing");
                    EndSwing();
                    break;
                case LassoState.ENEMY:
                    print("Lasso release of enemy");
                    break;
                case LassoState.NONE:
                default:
                    // Shouldn't be possible
                    print("Click release on invalid lasso state");
                    break;
                }
        }

        if (lasso_state == LassoState.WOUND_UP)
        {
            LassoWindup();
        }

        if (Input.GetKeyUp(jumpKey)) { EndJump(); }

        jump_hold_buffer_timer -= Time.deltaTime;
        jump_buffer_timer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse1) && can_Dash)
        {
            Dash();
            can_Dash = false;
            dash_Timer = dash_Cooldown;
        }

        if (!can_Dash)
        {
            dash_Timer -= Time.deltaTime;
            if (dash_Timer <= 0.0f)
            {
                can_Dash = true;
            }
        }
    }

    void FixedUpdate()
    {

        Run();
    }

    private Vector3 GetRelativeTangentVelocity()
    {
        Vector3 temp = transform.InverseTransformDirection(GetComponent<Rigidbody>().GetRelativePointVelocity(Vector3.zero));
        temp.y = 0;
        return temp;
    }

    private Vector3 GetTangentVelocity()
    {
        return transform.TransformDirection(GetRelativeTangentVelocity());
    }

    /**
     * Code for running momentum used from https://github.com/DawnosaurDev/platformer-movement/blob/main/Scripts/Run%20Only/PlayerRun.cs#L79
     */
    void Run()
    {
        _moveInput = cameraTransform.TransformDirection(_moveInput);
        _moveInput = Vector3.Dot(transform.right, _moveInput) * transform.right + Vector3.Dot(transform.forward, _moveInput) * transform.forward;
        Vector3 targetVelocity = _moveInput * max_walk_speed;
        float targetSpeed = targetVelocity.magnitude;
        if (targetSpeed > 0 && modelTransform != null)
        {
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, Quaternion.LookRotation(_moveInput, transform.up), Time.deltaTime * 8);
        }

        float accelRate;

        // Gets an acceleration value based on if we are accelerating (includes turning) 
        // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel_rate : decel_rate;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel_rate * accel_in_air_rate : decel_rate * decel_in_air_rate;


        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (conserve_momentum && 
            Mathf.Abs(GetTangentVelocity().magnitude) > Mathf.Abs(targetSpeed) && 
            Vector3.Dot(targetVelocity, GetComponent<Rigidbody>().GetPointVelocity(Vector3.zero)) > 0 && 
            Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }

        //Calculate difference between current velocity and desired velocity
        Vector3 speed_diff = targetVelocity - GetTangentVelocity();
        Vector3 movement = speed_diff * accelRate;
        //print("Adding a force of " +  movement);
        GetComponent<Rigidbody>().AddForce(movement);
    }

    void StartLassoWindup()
    {
        lasso_state = LassoState.WOUND_UP;
    }

    void LassoWindup()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lasso_dectection_distance);

        List<Vector3> indicator_positions = new List<Vector3>();

        Vector3 closest_point = Vector3.zero;
        float closest_distance = float.MaxValue;
        Collider closest_target_hit = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if (collider.tag == "Lasso Object")
            {
                // Note location of object for the indicators
                
                Vector3 viewport_point = Camera.main.WorldToViewportPoint(collider.transform.position);
                // Check if we are in the viewport, if we are, then add the collider positions for indicators.
                // Also mark the closest hit
                if (viewport_point.x > 0 && viewport_point.y > 0 &&
                    viewport_point.x < 1.0 && viewport_point.y < 1.0 &&
                    viewport_point.z > 0.0)
                {
                    indicator_positions.Add(collider.transform.position);
                    Vector3 viewport_point_centered = viewport_point - new Vector3(0.5f, 0.5f, 0.0f);
                    viewport_point_centered.z = 0;
                    if (viewport_point_centered.magnitude < closest_distance)
                    {
                        closest_target_hit = collider;
                        closest_distance = viewport_point_centered.magnitude;
                        closest_point = viewport_point;
                    }
                }
            }
        }


        // Place indicators for windup
        // 1. First, deactivate all previous indicators
        // 2. Then, activate all indicators that are necessary

        for (int i = 0; i < indicators.Count; i++)
        {
            indicators[i].SetActive(false);
        }
        int indicator_index = 0; // This is used to assign indicators. If it is ever >= indicators.Count, then we just stop doing indicators
        for (int i = 0; i < indicator_positions.Count; i++)
        {
            Vector3 viewport_point = Camera.main.WorldToViewportPoint(indicator_positions[i]);
            if (indicator_index < indicators.Count)
            {
                GameObject current_indicator = indicators[indicator_index];
                current_indicator.SetActive(true);
                current_indicator.transform.position = Camera.main.WorldToScreenPoint(indicator_positions[i]);
                //current_indicator.GetComponent<Image>().enabled = true;
                indicator_index++;
                // The indicator is in view, so place an indicator object on it
                if (Vector3.Distance(indicator_positions[i], transform.position) < lasso_reach_distance)
                {
                    if (viewport_point == closest_point)
                    {
                        // The lasso object is within range and the closest target, show a green indicator
                        current_indicator.GetComponent<Image>().color = targeted_color;
                    }
                    else
                    {
                        // The lasso object is within range, place a grey indicator if not the closest target
                        current_indicator.GetComponent<Image>().color = in_range_color;
                    }

                    if (closest_target_hit == null)
                    {
                        lasso_target = null;
                    }
                    else
                    {
                        lasso_target = closest_target_hit.gameObject;
                    }
                }

                if (Vector3.Distance(indicator_positions[i], transform.position) > lasso_reach_distance)
                {
                    // The lasso object is outside range, place a red indicator
                    current_indicator.GetComponent<Image>().color = out_of_range_color;
                    lasso_target = null;
                }
            }
        }
    }

    void EndLassoWindup()
    {
        if (lasso_target != null)
        {
            if (lasso_target.GetComponent<LassoableEnemy>() != null)
            {
                // TODO: Make this method (LassoEnemy) for the lassoable-enemy component script
                // which will attach the enemy to the player as now a child, so it follows the player
                // until it is thrown.

                // Or something else, just make the LassoEnemy work
            }
            if (lasso_target.GetComponent<Swingable>() != null)
            {
                StartSwing(lasso_target.transform.position);
            }
        }
        else
        {
            lasso_state = LassoState.NONE;
        }

        // Disable all indicators
        for (int i = 0; i < indicators.Count; i++)
        {
            indicators[i].SetActive(false);
        }
    }

    void StartSwing(Vector3 swing_position)
    {

        lasso_state = LassoState.SWING;
        swing_point = swing_position;
        swing_joint = player_root.gameObject.AddComponent<SpringJoint>();
        swing_joint.autoConfigureConnectedAnchor = false;
        swing_joint.connectedAnchor = swing_point;

        float distance_from_point = Vector3.Distance(player_root.position, swing_point);

        swing_joint.minDistance = distance_from_point * 0.6f;
        swing_joint.maxDistance = distance_from_point * 0.25f;

        current_rope_end_pos = lasso_throw_pos.position;

        swing_joint.spring = 4.5f;
        swing_joint.damper = 7f;
        swing_joint.massScale = 4.5f;

        lr.positionCount = 2;
    }
    
    void EndSwing()
    {
        lasso_state = LassoState.NONE; // Will be corrected on next update if wrong
        lr.positionCount = 0;
        Destroy(swing_joint);
    }

    void DrawRope()
    {
        if (!swing_joint)
        {
            return;
        }

        current_rope_end_pos = Vector3.Lerp(current_rope_end_pos, swing_point, Time.deltaTime * 8.0f);

        lr.SetPosition(0, lasso_throw_pos.position);
        lr.SetPosition(1, current_rope_end_pos);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartJump()
    {
        //_is_jumping = true;
        GetComponent<GravityObject>().gravity_mult = 1.0f;
        GetComponent<Rigidbody>().AddForce(transform.up * jump_impulse_force, ForceMode.Impulse);
        jump_hold_buffer_timer = jump_hold_buffer;
        jump_buffer_timer = 0;

    }

    void EndJump()
    {
        //_is_jumping = false;
        jump_buffer_timer = 0;
        //Vector3 velocity = GetComponent<Rigidbody>().velocity;
        //Vector3 up = Vector3.Project(velocity, transform.up);
        //float mult = (Vector3.Dot(up, transform.up) > 0 && jump_hold_buffer_timer > 0) ? 0.5f : 1;
        GetComponent<GravityObject>().gravity_mult = gravity_mult_on_jump_release;
    }

    void Dash()
    {
        //GetComponent<Rigidbody>().AddForce(temp.transform.forward * dash_force, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(modelTransform.forward * dash_force, ForceMode.Impulse);
    }
}
