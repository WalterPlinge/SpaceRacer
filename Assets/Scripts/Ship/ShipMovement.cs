/*
 * Script to handle the physics for the spaceship
 * Created 16/01/2018 by Lewis Wood
 * Last edited 24/02/2019 by Scott Davidson
 */

using Cinemachine;
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
		public float BoostAmount;
		public float BoostForce = 5.0f;
		public float BoostMaxAmount = 2.0f;
		public float BoostRechargeDelay;
		public float BoostMaxRechargeDelay = 2.0f;

		[Header("Physics Settings")]
		public float TerminalVelocity = 100.0f;
		public float HoverGravity = 20.0f;
		public float FallGravity = 80.0f;

		[Header("Animation Settings")]
		public Transform ShipRoll; // Used for rolling animation
		public CinemachineVirtualCamera Camera;
		public float Fov = 80.0f;
		public float FovDelta = 20.0f;
		public float FovBoostDelta = 10.0f;

		private float drag_;
		private bool isOnGround_;
		private RaycastHit rayInfo_;

		private Rigidbody rigidbody_;
		private PlayerInput input_;

		// Start is called before the first frame update
		void Start()
		{
			BoostAmount = BoostMaxAmount;

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
			{
				float sidewaysSpeed =
					Vector3.Dot(
						rigidbody_.velocity,
						transform.right);

				// Calculate and apply sideways friction (Slip is for drifting)
				sidewaysSpeed /= Time.fixedDeltaTime / Slip;

				Vector3 sideFriction = -transform.right * sidewaysSpeed;
				rigidbody_.AddForce(sideFriction, ForceMode.Acceleration);
			}

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

			// Apply boost
			{
				// Reset FOV
				UpdateFov(1.0f);

				// Boost available, player is boosting and ship is accelerating
				if (BoostAmount > 0.0f && input_.IsBoosting && input_.Thrust > 0.1f)
				{
					// Force based on amount left [1..BoostForce] (pow = curve)
					float boostAmountPercent = BoostAmount / BoostMaxAmount;
					float force = 1.0f +
						(BoostForce - 1.0f) *
						(1.0f - Mathf.Pow(boostAmountPercent, 1.0f));

					// Apply boost
					propulsion *= force;

					// Change FOV
					UpdateFov(boostAmountPercent);

					// Decrease boost
					BoostAmount -= Time.fixedDeltaTime;

					// Reset delay
					BoostRechargeDelay = 0.0f;
				}

				// Still waiting to recharge
				else if (BoostRechargeDelay < BoostMaxRechargeDelay)
				{
					BoostRechargeDelay += Time.fixedDeltaTime;
				}

				// Recharge boost at 1/3 speed
				else if (BoostAmount < BoostMaxAmount)
				{
					BoostAmount += Time.fixedDeltaTime / 3.0f;
				}
			}

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
				Quaternion.Slerp(
					rigidbody_.rotation,
					rotation,
					Time.deltaTime * 10.0f));

			// Calculate roll angle
			float angle = MaxRollAngle * -input_.Steering;

			// Use ShipMeshFix to deal with broken model transform
			Quaternion roll = Quaternion.Euler(0.0f, 0.0f, angle);

			// Apply roll to mesh (Cosmetic)
			ShipRoll.localRotation =
				Quaternion.Slerp(
					ShipRoll.localRotation,
					roll,
					Time.deltaTime * 10.0f);
		}

		void UpdateFov(float BoostPercent)
		{
			float amount =
				Fov +
				FovDelta * Speed / TerminalVelocity +
				FovBoostDelta * (1.0f - BoostPercent);

			Camera.m_Lens.FieldOfView = Mathf.Lerp(
				Camera.m_Lens.FieldOfView,
				amount,
				Time.fixedDeltaTime * 10.0f);
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
