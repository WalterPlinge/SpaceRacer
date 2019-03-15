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
		public float DriveForce = 16.0f;
		public float SteerForce = 2.5f;
		public float SlowingFactor = 0.99f;
		public float BrakingFactor = 0.95f;
		public float MaxRollAngle = 30.0f;
		public float Slip = 1.0f;

		[Header("Hover Settings")]
		public float TargetHoverHeight = 0.5f;
		public float MaxGroundDistance = 4.0f;
		public float HoverForce = 300.0f;
		public LayerMask WhatIsGround;
		public PidController HoverPid; // Used to stop hover ocillation

		[Header("Boost Settings")]
		public float BoostAmount; //amount of boost available to the player
		public float BoostForce = 3.0f;
		public float MaxBoost = 2.0f; //maximum amount of boost that can be used
		public float BoostTimer; //timer to count how long it has been since the player last hit the boost button
		public float BoostTimeLimit = 2.0f; // ?
		
		[Header("Physics Settings")]
		public float TerminalVelocity = 100.0f;
		public float HoverGravity = 20.0f;
		public float FallGravity = 80.0f;

		[Header("Animation Settings")]
		public Transform Ship; // Used for banking animation
		public Transform ShipFix; // Fix banking rotation

		private float drag_;
		private bool isOnGround_;
		private RaycastHit rayInfo_;

		private Rigidbody rigidbody_;
		private PlayerInput input_;

		// Start is called before the first frame update
		void Start()
		{
			BoostAmount = MaxBoost; //give the player all the available boost

			//// Potentially switch to tv = df / d (easier to switch planets?)
			drag_ = DriveForce / TerminalVelocity;

			rigidbody_ = GetComponent<Rigidbody>();
			input_ = GetComponent<PlayerInput>();
		}

		// All physics calculations handled inside FIxedUpdate
		void FixedUpdate()
		{
			// Raycast
			Ray ray = new Ray(transform.position, -transform.up);
			isOnGround_ = Physics.Raycast(ray, out rayInfo_,
					MaxGroundDistance,
					WhatIsGround);

			// Calculate the forces to be applied to the ship
			CalculateHover();
			CalculatePropulsion();
			UpdateRotation();
		}

		void CalculateHover()
		{
			// Ground normal
			Vector3 normal = isOnGround_
				? rayInfo_.normal
				: Vector3.up;

			// Hover and gravity default values
			Vector3 hover = Vector3.zero;
			Vector3 gravity = -normal;

			// If on ground, set hover amount using PID
			if (isOnGround_)
			{
				float height = rayInfo_.distance;
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
			// Current speed in forward direction
			Speed = Vector3.Dot(rigidbody_.velocity, transform.forward);

			// Calculate steering force and apply it to body
			{
				float direction = Speed < -1.0f ? -1.0f : 1.0f;
				float force = direction * SteerForce * input_.Steering;

				// Use amount of yaw torque
				force -= Vector3.Dot(rigidbody_.angularVelocity, transform.up);
				
				rigidbody_.AddRelativeTorque(
					0.0f, force, 0.0f,
					ForceMode.VelocityChange);
			}
       
			
			
			// Calculate speed in the left/right direction (right is positive)
			float sidewaysSpeed =
				Vector3.Dot(
					rigidbody_.velocity,
					transform.right);
			
			// Calculate and apply sideways friction (Slip is for drifting)
			sidewaysSpeed /= Time.fixedDeltaTime / Slip;

			Vector3 sideFriction = -transform.right * sidewaysSpeed;
			rigidbody_.AddForce(sideFriction, ForceMode.Acceleration);

			
			
			// If  not accelerating, slow the ship
			if (input_.Thrust <= 0f)
				rigidbody_.velocity *= SlowingFactor;

			// If not on ground, exit
			if (!isOnGround_)
				return;

			// If player is braking, slow the ship
			if (input_.IsBraking)
				rigidbody_.velocity *= BrakingFactor;

			
			
			// Calculate propulsion
			float propulsion = DriveForce * input_.Thrust;

            //      Boost Code      //
            ///////////////////////////////////////////////////////
		    {
		        // Apply boost if player has boost available and is pressing the boost button
		        if (BoostAmount > 0.0f && input_.IsBoosting && Speed > 10)
		        {

                    //reset the boost timer to 0
                    //this means that it has been 0 seconds since the player boosted
		            BoostTimer = 0;

		            //reduce the amount of boost time available
                    BoostAmount -= Time.fixedDeltaTime;

		            //multiply the propulsion by the boost force
                    propulsion *= BoostForce; 

                    //trigger boost effect
                    //*Boost effect code goes here*//

		        }
                
               
                //if the player is not boosting, add delta time to the boost timer
                //this counts how long it has been since the player last used their boost
		        if (!input_.IsBoosting || BoostAmount <= 0.0f)
		        {
		            BoostTimer += Time.fixedDeltaTime;
		        }

		        // if the boost is not at max value, slowly recharge the amount of boost the player has
                //this is done after the player has not boosted for 3 seconds
		        if (BoostTimer >= 3.0f && BoostAmount < MaxBoost)
		        {
		            BoostAmount += Time.fixedDeltaTime * 0.3f;
		        }

		    }

            //////////////////////////////////////////////////////
            
		    // Apply drag
			float clampedSpeed = Mathf.Clamp(Speed, 0.0f, TerminalVelocity);
			float dragAmount = drag_ * clampedSpeed;
			propulsion -= dragAmount;

			rigidbody_.AddForce(
				propulsion * transform.forward,
				ForceMode.Acceleration);

			

			// Calculate and apply strafe thrust
			Vector3 strafe = transform.right * DriveForce * input_.Strafe;
			rigidbody_.AddForce(strafe, ForceMode.Impulse);
		}

		void UpdateRotation()
		{
			// Ground normal
			Vector3 normal = isOnGround_
				? rayInfo_.normal
				: Vector3.up;

			// Get rotation using forward vector parallel to ground
			Vector3 forward = Vector3.ProjectOnPlane(transform.forward, normal);
			Quaternion rotation = Quaternion.LookRotation(forward, normal);

			// Move ship rigid body to match this over time
			rigidbody_.MoveRotation(
				Quaternion.Lerp(
					rigidbody_.rotation,
					rotation,
					Time.deltaTime * 10.0f));
			
			
			
			// Calculate just roll component (-90 x and angle y to fix model)
			float angle = MaxRollAngle * input_.Steering;

			// Use ShipMeshFix to deal with broken model transform
			Quaternion roll =
				ShipFix.rotation *
				Quaternion.Euler(0.0f, angle, 0.0f);

			// Apply roll to mesh (Cosmetic)
			Ship.rotation =
				Quaternion.Lerp(
					Ship.rotation,
					roll,
					Time.deltaTime * 10.0f);
		}

		void OnCollision(Collision collision)
		{
			// Apply downwards force on collision with wall
			if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
			{
				// Counter upwards force from collision
				Vector3 upwardForce =
					Vector3.Dot(
						collision.impulse,
						transform.up) *
					transform.up;
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
