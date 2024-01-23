using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    Transform cameraTransform;
    [Header("References")]
    public Transform player_root;
    public Transform modelTransform;
    public LineRenderer lr;
    public Transform lasso_throw_pos;
    public LayerMask swingable;


    [Header("Movement")]
    public float max_speed = 15.0f;
    public float accel_rate = (50.0f * 0.5f) / 15.0f;
    public float decel_rate = 40.0f;
    public float accel_in_air_rate = 0.8f;
    public float decel_in_air_rate = 0.8f;
    public bool conserve_momentum = true;

    [Header("Swinging")]
    private float swing_reach_distance = 25f;
    private Vector3 swing_point;
    private SpringJoint swing_joint;
    private Vector3 current_rope_end_pos;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;
    public float LastOnGroundTime { get; private set; }

    private Vector3 _moveInput;


    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(horizontal, 0, vertical).normalized;

        LastOnGroundTime -= Time.deltaTime;
        if (GetComponent<GravityObject>().IsOnGround())
        {
            LastOnGroundTime = 0.1f;
        }

        if (Input.GetKeyDown(swingKey)) { StartSwing(); }
        if (Input.GetKeyUp(swingKey)) { EndSwing(); }

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
        Vector3 targetVelocity = _moveInput * max_speed;
        float targetSpeed = targetVelocity.magnitude;
        if (targetSpeed > 0 && modelTransform != null)
        {
            modelTransform.rotation = Quaternion.LookRotation(_moveInput, transform.up);
        }

        float accelRate;

        // Gets an acceleration value based on if we are accelerating (includes turning) 
        // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel_rate : decel_rate;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel_rate * accel_in_air_rate : decel_rate * decel_in_air_rate;


        //Not used since no jump implemented here, but may be useful if you plan to implement your own
        /* 
		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion
		*/


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

        
        GetComponent<Rigidbody>().AddForce(movement);
    }

    void StartSwing()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, swing_reach_distance, swingable))
        {
            swing_point = hit.point;
            swing_joint = player_root.gameObject.AddComponent<SpringJoint>();
            swing_joint.autoConfigureConnectedAnchor = false;
            swing_joint.connectedAnchor = swing_point;

            float distance_from_point = Vector3.Distance(player_root.position, swing_point);

            swing_joint.minDistance = distance_from_point * 0.8f;
            swing_joint.maxDistance = distance_from_point * 0.25f;

            current_rope_end_pos = lasso_throw_pos.position;

            swing_joint.spring = 4.5f;
            swing_joint.damper = 7f;
            swing_joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }
    
    void EndSwing()
    {
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
}
