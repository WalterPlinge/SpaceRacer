/*
 * Script to handle the physics for the spaceship
 * Created 16/01/2018 by Lewis Wood
 * Last edited 13/02/2019 by Scott Davidson
 */

using Assets.Scripts.Ship;
using UnityEngine;

public class ShipPhysics : MonoBehaviour
{
	// Velocity settings
	public float Speed;                 // The forward speed of the ship
	public float Slipperiness;			// Friction for drifting
	public float DriveForce;			// The force that the engine produces
	public float SlowingFactor;			// The percentage of velocity maintained when not accelerating
	public float BrakingFactor;			// The percentage of velocity maintained when braking
	public float RollingAngle;			// The max angle for banking while turning

	// Hover settings
	public float HoverHeight;			// The target hover height
	public float MaxGroundDistance;		// The max distance to detect ground
	public float HoverForce;			// Force applied towards hoverHeight, lower = bouncier elevation changes
	public PidController HoverPid;		// PID controller prevents oscillation when hovering at set height

	public bool IsOnGround;				// Whether ship is on the ground
	public LayerMask WhatIsGround;      // Layer used for ground
	public RaycastHit RayHitInfo;		// Information about raycast

	// Physics settings
	public float TerminalVelocity;		// Max speed of ship
	public float HoverGravity;			// Gravity when ground is detected
	public float FallGravity;			// Gravity when ground not detected
	public float Drag;					// Air resistance in forward direction

	// Components
	private Controls input_;			// Reference to the player's input
	private Rigidbody shipRigidbody_;   // Reference to the ship's rigid body
	private Transform ship_;            // Reference to the ship's transform

	// When entity is awoken (runs first time)
    void Awake()
	{
		// Get the references to the Rigidbody and the player's input
		ship_ = GetComponent<Transform>();
		shipRigidbody_ = GetComponent<Rigidbody>();
		input_ = GetComponent<Controls>();

		// Velocity settings
		Slipperiness = 1.0f;
		DriveForce = 16.0f;
		SlowingFactor = 0.99f;
		BrakingFactor = 0.95f;
		RollingAngle = 30.0f;

		// Hover settings
		HoverHeight = 0.5f;
		MaxGroundDistance = 4.0f;
		HoverForce = 128.0f;
		HoverPid = new PidController();

		WhatIsGround = LayerMask.GetMask("Ground");
		IsOnGround = Physics.Raycast(new Ray(ship_.position, -ship_.up), out RayHitInfo, MaxGroundDistance, WhatIsGround);

		// Physics settings
		TerminalVelocity = 128.0f;
		HoverGravity = 32.0f;
		FallGravity = 64.0f;
		Drag = DriveForce / TerminalVelocity;
	}

	void FixedUpdate() // all physics calculations handled inside FIxedUpdate
	{
		// Calculate the current speed by using the dot product
		// This tells us how much of the ship's velocity is in the forward direction
		Speed = Vector3.Dot(shipRigidbody_.velocity, transform.forward);

		// Raycast
		IsOnGround = Physics.Raycast(new Ray(ship_.position, -ship_.up), out RayHitInfo, MaxGroundDistance, WhatIsGround);

		// Calculate the forces to be applied to the ship
		UpdateForces();
		UpdateRotation();
		CalculateMovement();
	}

	void UpdateForces()
	{
		// Ground normal
		Vector3 normal = IsOnGround ? RayHitInfo.normal : Vector3.up;

		// Hover and gravity default values
		Vector3 hover = Vector3.zero;
		Vector3 gravity = -normal;

		// Hover will be dependant on offset from target height (PID.Seek)
		if (IsOnGround)
		{
			float distance = RayHitInfo.distance;
			hover = normal * HoverForce * HoverPid.Seek(HoverHeight, distance);
			gravity *= HoverGravity * distance;
		}
		else
			gravity *= FallGravity;

		// Apply forces
		shipRigidbody_.AddForce(hover, ForceMode.Force);
		shipRigidbody_.AddForce(gravity, ForceMode.Acceleration);
	}

    void UpdateRotation()
	{
		// Ground normal
		Vector3 normal = IsOnGround ? RayHitInfo.normal : Vector3.up;

        // Get rotation using forward vector parallel to ground normal
	    Vector3 forward = Vector3.ProjectOnPlane(ship_.forward, normal).normalized;
		Quaternion rotation = Quaternion.LookRotation(forward, normal);

		// Apply roll
		float angle = RollingAngle * -input_.steering;
		rotation *= Quaternion.Euler(0.0f, 0.0f, angle);

		// Rotate ship by interpolating between old and new rotations
		shipRigidbody_.MoveRotation(Quaternion.Slerp(ship_.rotation, rotation, Time.fixedDeltaTime * 10.0f));
    }

    void CalculateMovement()
	{
		// Calculate the yaw torque based on the steering and the current angular velocity
		float rotationTorque = input_.steering - shipRigidbody_.angularVelocity.y;

		// Apply the torque to the ship's Y axis
		shipRigidbody_.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);
       
        //calculate the current sideways speed of the ship, this tells us how much of the ship's velocity is in the right or left direction
		float sidewaysSpeed = Vector3.Dot(shipRigidbody_.velocity, transform.right);

        //calculate the desired amount of friction to apply to the side of the ship, this is what keeps the ship from drifting in to walls during turns
        //slipperiness is used to include drifting in the game
		Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime / Slipperiness);

        //apply the sideways friction to the ship
        shipRigidbody_.AddForce(sideFriction, ForceMode.Acceleration);

        //if the player is not accelerating, slow the ship slightly
		if (input_.acceleration <= 0f)
		{
			shipRigidbody_.velocity *= SlowingFactor;
		}
        
        //if the ship isn't on the ground, do not brake or slow the ship
		if (!IsOnGround)
		{
			return;
		}

        //apply braking force to the ship if the player is pressing the brakes
		if (input_.isBraking)
		{
			shipRigidbody_.velocity *= BrakingFactor;

		}

        //calculate the amount of propulsion in each direction and apply it to the ship
		float propulsion = DriveForce * input_.acceleration - Drag * Mathf.Clamp(Speed, 0f, TerminalVelocity);
        float sidewaysPropulsion = DriveForce * input_.thruster;
		shipRigidbody_.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
        shipRigidbody_.AddForce(transform.right * sidewaysPropulsion, ForceMode.Impulse);
	}

    void OnCollision(Collision collision)
    {
        //if the ship has collided with an object on the Wall layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //calculate how much upwards impulse is generated and then push the vehicle down by that amount to keep it stuck to the track instead of popping over the wall
            Vector3 upwardForceFromCollision = Vector3.Dot(collision.impulse, transform.up) * transform.up;
            shipRigidbody_.AddForce(-upwardForceFromCollision, ForceMode.Impulse);
        }
    }

    //get how fast the ship is going as a percentage (could be used for speedometer, we could just use the actual speed instead of just the percentage speed if we need to)
    public float GetSpeedPercentage()
    {
        return shipRigidbody_.velocity.magnitude / TerminalVelocity;
    }
}
