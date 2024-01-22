using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    Transform cameraTransform;
    public Transform modelTransform;

    public float max_speed = 15.0f;
    public float accel_rate = (50.0f * 0.5f) / 15.0f;
    public float decel_rate = 40.0f;
    public float accel_in_air_rate = 0.8f;
    public float decel_in_air_rate = 0.8f;
    public bool conserve_momentum = true;

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


        //print("Relative speed is: " + GetRelativeTangentVelocity().magnitude);
        //Vector3 target_speed = new Vector3(horizontal, 0, vertical).normalized * speed;
        //Vector3 relative_vel = GetComponent<Rigidbody>().GetRelativePointVelocity(Vector3.zero);
        //print("Relative velocity is: " + relative_vel);
        //relative_vel.y = 0;
        //
        //moveAmount = speed_diff * accel_rate;


        //bool jump = Input.GetButtonDown("Jump");
        //Vector3 direction = new Vector3 (horizontal, 0, vertical).normalized;
        //if (jump && GetComponent<GravityObject>().IsOnGround())
        //{
        //    print("Jump pressed");
        //    direction += new Vector3(0, jump_impulse, 0);
        //}
        //moveAmount = direction;

        //bool released = Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical");
        //if (released && moveAmount.magnitude <= 0)
        //{
        //    print("Magnitude <= 0");
        //    GetComponent<Rigidbody>().AddForce(-GetComponent<Rigidbody>().GetAccumulatedForce());
        //    GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    moveAmount = Vector3.zero;
        //}
        //Vector3 targetMoveAmount = direction * speed;

        //moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

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


        //Vector3 up_basis = transform.up;
        //Vector3 forward_basis = Vector3.Cross(up_basis, cameraTransform.transform.right).normalized;
        //Vector3 right_basis = Vector3.Cross(forward_basis, up_basis).normalized;

        //print("Current Vel Target: " + targetVelocity);

        //print(("Basises: \n Up: " + up_basis + "\n Forward: " + forward_basis + "\n Right: " + right_basis));
        //targetVelocity = new Vector3(
        //    right_basis.x * targetVelocity.x + right_basis.y * targetVelocity.y + right_basis.z * targetVelocity.z,
        //    up_basis.x * targetVelocity.x + up_basis.y * targetVelocity.y + up_basis.z * targetVelocity.z,
        //    forward_basis.x * targetVelocity.x + forward_basis.y * targetVelocity.y + forward_basis.z * targetVelocity.z
        //);

        //print("New Vel Target: " + targetVelocity);

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
}
