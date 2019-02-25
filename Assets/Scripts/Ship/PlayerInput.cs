using UnityEngine;

namespace Assets.Scripts.Ship
{
	public class PlayerInput : MonoBehaviour
	{

		public string ForwardAxisName;
		public string TurningAxisName;
		public string ThrusterAxisName;
		public string BrakeKey;

		[HideInInspector] public float Acceleration;
		[HideInInspector] public float Thruster;
		[HideInInspector] public float Steering;
		[HideInInspector] public bool IsBraking;

		void Reset()
		{
			ForwardAxisName = "Vertical";
			TurningAxisName = "Horizontal";
			ThrusterAxisName = "Strafe";
			BrakeKey = "Brake";
		}

		void Update()
		{
			// If the player hits the escape key inside a build, close the application
			if (Input.GetButtonDown("Exit") && !Application.isEditor)
				Application.Quit();

			// Get values from input class
			Acceleration = Input.GetAxis(ForwardAxisName);

			Steering = Input.GetAxis(TurningAxisName);
			// Make steering exponential
			//Steering *= Mathf.Abs(Steering);

			Thruster = Input.GetAxis(ThrusterAxisName);

			IsBraking = Input.GetButton(BrakeKey);
		}
	}
}
