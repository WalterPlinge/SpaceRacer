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
			if (UnityEngine.Input.GetButtonDown("Exit") && !Application.isEditor)
				Application.Quit();

			// Get values from input class
			Acceleration = UnityEngine.Input.GetAxis(ForwardAxisName);
			Thruster = UnityEngine.Input.GetAxis(ThrusterAxisName);
			Steering = UnityEngine.Input.GetAxis(TurningAxisName);
			IsBraking = UnityEngine.Input.GetButton(BrakeKey);
		}
	}
}
