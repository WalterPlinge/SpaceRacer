// The class handles calculating a desired value based on a PID algorithm

using UnityEngine;

namespace Assets.Scripts.Ship
{
	[System.Serializable]
	public class PidController
	{
		// Our PID coefficients for tuning the controller
		public float Kp = .8f;
		public float Ki = .0002f;
		public float Kd = .2f;
		public float Min = -1;
		public float Max = 1;

		// Variables to store values between calculations
		private float integral_;
		private float lastProportional_;

		// Returns value that will move current toward goal
		public float Seek(float seekValue, float currentValue)
		{
			float deltaTime = Time.fixedDeltaTime;
			float proportional = seekValue - currentValue;

			float derivative = (proportional - lastProportional_) / deltaTime;
			integral_ += proportional * deltaTime;
			lastProportional_ = proportional;

			//This is the actual PID formula. This gives us the value that is returned
			float value = Kp * proportional + Ki * integral_ + Kd * derivative;
			value = Mathf.Clamp(value, Min, Max);

			return value;
		}
	}
}
