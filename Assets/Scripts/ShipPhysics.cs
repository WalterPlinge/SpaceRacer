/*
 * Script to handle the physics for the spaceship
 * Created 16/01/2018 by Lewis Wood
 * Last edited 10/02/2019 by Lewis Wood
 */

using UnityEngine;

public class ShipPhysics : MonoBehaviour
{
	public float speed; // the forward speed fo the ship

	public float slipperiness = 1.0f;  // DO NOT SET TO ZERO!!!, adds drift when increased past 1.0f

	//velocity settings
	public float driveforce = 17.0f;  //the force that the engine produces
	public float slowingFactor = 0.99f; // The percentage of velocity to maintain when not accelerating
	public float brakingFactor = 0.95f; // The percentage of velocity the ship maintains when braking
	public float rollingAngle = 45.0f;    // The angle that the ship will bank to while turning

	//hover settings
	public float hoverHeight = 0.5f;        // The height the ship aims to hover at
	public float maxGroundDistance = 4.0f;    // The distance the ship can be off the ground without falling
	public float hoverForce = 300.0f;         // Force used to move ship towards hoverHeight , reducing this increases bounciness when transitioning between different heights
	public LayerMask whatIsGround;          // Layer used for ground
	public PIDController hoverPID;          // PID controller to handle ship's hovering (Proportional Integral Derivative controller, prevents oscillation when trying to hover at set height)

	//physics settings
	public Transform ship;                  // Reference to the ship
	public float terminalVelocity = 100.0f;   // Sets the max speed of the ship
	public float hoverGravity = 20.0f;        // Gravity to apply when the ship is near the ground
	public float fallGravity = 80.0f;         // Gravity to apply when the ship can not detect the ground

	Rigidbody shipRigidbody_;   // Reference to the ship's rigid body
	Controls input_;            // Reference to the player's input
	float drag_;                // The air resistance the ship recieves in the forward direction
	bool isOnGround_;           // A flag determining if the ship is currently on the ground

    

    void Start()
	{
		// Get the references to the Rigidbody and the player's input
		shipRigidbody_ = GetComponent<Rigidbody>();
		input_ = GetComponent<Controls>();

        //set the ground to the "Ground" layer
       // whatIsGround = LayerMask.NameToLayer("Ground");

		//calculate the ship's drag value
		drag_ = driveforce / terminalVelocity;
	}

	void FixedUpdate() // all physics calculations handled inside FIxedUpdate
	{
		// Calculate the current speed by using the dot product
		// This tells us how much of the ship's velocity is in the forward direction
		speed = Vector3.Dot(shipRigidbody_.velocity, transform.forward);

		// Calculate the forces to be applied to the ship
		CalculateHover();
		CalculateMovement();
	}

    void CalculateHover()
    {
		// Calculate new forces
		// Calculate new rotation
		// Apply both

        Vector3 groundNormal; // variable that holds the normal of the ground, points "up" from the surface declared as ground

        // Cast ray directly down from the ship
        Ray ray = new Ray(ship.position, -ship.up);

        //declare a variable that holds the result of the raycast
	    isOnGround_ = Physics.Raycast(ray, out RaycastHit hitInfo, maxGroundDistance, whatIsGround);

        // if the ship detects the ground
        if (isOnGround_)
        {
            //determine how high off the ground we are
            float height = hitInfo.distance;
            //save the normal of the ground
            groundNormal = hitInfo.normal;

            // Use PID controller to find hover force amount required
            float forceAmount = hoverPID.Seek(hoverHeight, height);

            //calculate the total amount of hover force required based on the normal of the ground
            Vector3 force = groundNormal * hoverForce * forceAmount;

            //calculate the force and direction of gravity (gravity is not always based on the terrain)
            Vector3 gravity = -groundNormal * hoverGravity * height;

            // Apply hover and gravity forces
            shipRigidbody_.AddForce(force, ForceMode.Force);
            shipRigidbody_.AddForce(gravity, ForceMode.Acceleration);

        }
        else //if the raycast did not detect the ground
        {
            //use up to represent the ground normal (this will cause the ship to self right if it flips over)
            groundNormal = Vector3.up;

            //calculate and apply a stronger gravity force to pull the ship towards the ground
            Vector3 gravity = -groundNormal * fallGravity;
            shipRigidbody_.AddForce(gravity, ForceMode.Acceleration);
        }

		// Get ground rotation using forward vector parallel to ground normal
	    Vector3 forward = Vector3.ProjectOnPlane(ship.forward, groundNormal);
	    Quaternion direction = Quaternion.LookRotation(forward, groundNormal);

		// Get roll rotation
	    Quaternion banking = Quaternion.Euler(0, 0, -rollingAngle * input_.steering);

		// Rotate ship by interpolating between old and new rotations
	    Quaternion newRotation = Quaternion.Slerp(shipRigidbody_.rotation, direction * banking, Time.fixedDeltaTime * 10.0f);
	    shipRigidbody_.MoveRotation(newRotation);
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
		Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime / slipperiness);

        //apply the sideways friction to the ship
        shipRigidbody_.AddForce(sideFriction, ForceMode.Acceleration);

        //if the player is not accelerating, slow the ship slightly
		if (input_.acceleration <= 0f)
		{
			shipRigidbody_.velocity *= slowingFactor;
		}
        
        //if the ship isn't on the ground, do not brake or slow the ship
		if (!isOnGround_)
		{
			return;
		}

        //apply braking force to the ship if the player is pressing the brakes
		if (input_.isBraking)
		{
			shipRigidbody_.velocity *= brakingFactor;

		}

        //calculate the amount of propulsion in each direction and apply it to the ship
		float propulsion = driveforce * input_.acceleration - drag_ * Mathf.Clamp(speed, 0f, terminalVelocity);
        float sidewaysPropulsion = driveforce * input_.thruster;
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
        return shipRigidbody_.velocity.magnitude / terminalVelocity;
    }
}
