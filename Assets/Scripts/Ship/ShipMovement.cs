/*
 * Script to handle the physics for the spaceship
 * Created 16/01/2018 by Lewis Wood
 * Last edited 24/02/2019 by Scott Davidson
 */

using UnityEngine;

namespace Assets.Scripts.Ship
{
	public class ShipMovement : MonoBehaviour
	{
		// The current forward speed of the ship
		public float Speed;

		[Header("Drive Settings")]
		public float DriveForce = 17.0f;
		public float SlowingFactor = 0.99f;
		public float BrakingFactor = 0.95f;
		public float MaxRollAngle = 30.0f;
		public float Slip = 1.0f;

		[Header("Hover Settings")]
		public float TargetHoverHeight = 1.0f;
		public float MaxGroundDistance = 4.0f;
		public float HoverForce = 300.0f;
		public LayerMask WhatIsGround;
		public PidController HoverPid; // Used to stop hover ocillation
		
		[Header("Physics Settings")]
		public float TerminalVelocity = 100.0f;
		public float HoverGravity = 20.0f;
		public float FallGravity = 80.0f;

		[Header("Animation Settings")]
		public Transform ShipMesh; // Used for banking animation

		private float drag_;
		private bool isOnGround_;
		private RaycastHit rayHitInfo_;

		private Rigidbody rigidbody_;
		private PlayerInput input_;

		// Start is called before the first frame update
		void Start()
		{
			rigidbody_ = GetComponent<Rigidbody>();
			input_ = GetComponent<PlayerInput>();

			drag_ = DriveForce / TerminalVelocity;
		}

		void FixedUpdate() // all physics calculations handled inside FIxedUpdate
		{
			// Calculate the current speed by using the dot product
			// This tells us how much of the ship's velocity is in the forward direction
			Speed = Vector3.Dot(rigidbody_.velocity, transform.forward);

			// Raycast
			Ray ray = new Ray(transform.position, -transform.up);
			isOnGround_ = Physics.Raycast(ray, out rayHitInfo_, MaxGroundDistance, WhatIsGround);

			// Calculate the forces to be applied to the ship
			CalculateHover();
			UpdateRotation();
			CalculatePropulsion();
		}

		void CalculateHover()
		{
			// Ground normal
			Vector3 normal = isOnGround_ ? rayHitInfo_.normal : Vector3.up;

			// Hover and gravity default values
			Vector3 hover = Vector3.zero;
			Vector3 gravity = -normal;

			// If on ground, set hover amount using PID
			if (isOnGround_)
			{
				float height = rayHitInfo_.distance;
				float amount = HoverPid.Seek(TargetHoverHeight, height);

				hover = normal * HoverForce * amount;
				gravity *= HoverGravity * height;
			}
			else
				gravity *= FallGravity;

			// Apply forces
			rigidbody_.AddForce(hover, ForceMode.Acceleration);
			rigidbody_.AddForce(gravity, ForceMode.Acceleration);
		}

		void CalculatePropulsion()
		{
			// Calculate steering force (yaw torque) and apply it to body
			float steeringForce = input_.Steering - rigidbody_.angularVelocity.y;
			rigidbody_.AddRelativeTorque(0.0f, steeringForce, 0.0f, ForceMode.VelocityChange);
       
			
			
			// Calculate speed in the left/right direction (right is positive)
			float sidewaysSpeed = Vector3.Dot(rigidbody_.velocity, transform.right);
			
			// Calculate and apply sideways friction to counter sideways speed (Slip is for drifting)
			Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime / Slip);
			rigidbody_.AddForce(sideFriction, ForceMode.Acceleration);

			
			
			// If  not accelerating, slow the ship
			if (input_.Acceleration <= 0f)
				rigidbody_.velocity *= SlowingFactor;

			// If not on ground, exit
			if (!isOnGround_)
				return;

			// If player is braking, slow the ship
			if (input_.IsBraking)
				rigidbody_.velocity *= BrakingFactor;

			
			
			// Calculate and apply propulsion force by the amount of thrust and subtract drag amount
			float dragAmount = drag_ * Mathf.Clamp(Speed, 0.0f, TerminalVelocity);
			float propulsion = DriveForce * input_.Acceleration - dragAmount;
			rigidbody_.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
		}

		void UpdateRotation()
		{
			// Ground normal
			Vector3 normal = isOnGround_ ? rayHitInfo_.normal : Vector3.up;

			// Get rotation using forward vector parallel to ground
			Vector3 forward = Vector3.ProjectOnPlane(transform.forward, normal);
			Quaternion rotation = Quaternion.LookRotation(forward, normal);

			// Move ship rigid body to match this over time
			rigidbody_.MoveRotation(Quaternion.Lerp(rigidbody_.rotation, rotation, Time.deltaTime * 10.0f));
			
			
			
			// Calculate just roll component
			float angle = MaxRollAngle * -input_.Steering;
			Quaternion roll = transform.rotation * Quaternion.Euler(0.0f, 0.0f, angle);

			// Apply roll to mesh (Cosmetic)
			ShipMesh.rotation = Quaternion.Lerp(ShipMesh.rotation, roll, Time.deltaTime * 10.0f);
		}

		void OnCollision(Collision collision)
		{
			// Apply downwards force on collision with wall
			if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				// Counter upwards force from collision
				Vector3 upwardForce = Vector3.Dot(collision.impulse, transform.up) * transform.up;
				rigidbody_.AddForce(-upwardForce, ForceMode.Impulse);
			}
		}

		// How fast the ship is going as percentage of terminal velocity
		public float GetSpeedPercentage()
		{
			return rigidbody_.velocity.magnitude / TerminalVelocity;
		}
	}
}
