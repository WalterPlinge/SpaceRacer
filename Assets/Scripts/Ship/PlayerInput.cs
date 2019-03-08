using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ship
{
	public class PlayerInput : MonoBehaviour
	{

		public string ThrustAxis;
		public string SteeringAxis;
		public string StrafeAxis;
		public string BrakeKey;
		public string BoostKey;

		[HideInInspector] public float Thrust;
		[HideInInspector] public float Strafe;
		[HideInInspector] public float Steering;
		[HideInInspector] public bool IsBraking;
		[HideInInspector] public bool IsBoosting;

		void Reset()
		{
			ThrustAxis = "Thrust";
			SteeringAxis = "Steering";
			StrafeAxis = "Strafe";
			BrakeKey = "Brake";
			BoostKey = "Boost";
		}

		void Update()
		{
			// If the player hits the escape key inside a build, close the application
			if (Input.GetButtonDown("Exit") && !Application.isEditor && SceneManager.GetSceneByName("EscapeMenu").isLoaded == false)
            {
                SceneManager.LoadSceneAsync("EscapeMenu", LoadSceneMode.Additive);
                Time.timeScale = 0;
            }

			// Get values from input class
			Thrust = Input.GetAxis(ThrustAxis);

			Steering = Input.GetAxis(SteeringAxis);
			// Make steering exponential
			//Steering *= Mathf.Abs(Steering);

			Strafe = Input.GetAxis(StrafeAxis);

			IsBraking = Input.GetButton(BrakeKey);

			IsBoosting = Input.GetButton(BoostKey);
		}
	}
}
