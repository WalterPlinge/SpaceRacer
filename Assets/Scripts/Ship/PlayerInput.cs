using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ship
{
	public class PlayerInput : MonoBehaviour
	{

        private bool xbox = false;
        private bool playstation = true;

        public string playstationThrustFix;
		public string ThrustAxis;
		public string SteeringAxis;
		public string StrafeAxis;
		public string BrakeKey;
		public string BoostKey;

		 public float Thrust;
		[HideInInspector] public float Strafe;
		[HideInInspector] public float Steering;
		[HideInInspector] public bool IsBraking;
		[HideInInspector] public bool IsBoosting;

		void Awake()
		{
            //find out if the player is using an xbox or playstation controller
            string[] names = Input.GetJoystickNames();
            for (int x = 0; x < names.Length; x++)
            {
                print(names[x].Length);
                if (names[x].Length == 19)
                {
                    print("PS4 controller is connected");
                    playstation = true;
                    xbox = false;
                }
                if (names[x].Length == 33)
                {
                    print("Xbox controller is connected");
                    playstation = false;
                    xbox = true;
                }
            }

            if (xbox) //if an xbox controller is connected
            {
                ThrustAxis = "Xbox Thrust";
                SteeringAxis = "Xbox Steering";
                StrafeAxis = "Xbox Strafe";
                BrakeKey = "Xbox Brake";
                BoostKey = "Xbox Boost";
            }

            else if (playstation) // if a playstation controller is connected
            {
                playstationThrustFix = "Playstation L2";
                ThrustAxis = "Playstation R2";
                SteeringAxis = "Xbox Steering";
                StrafeAxis = "Xbox Strafe";
                BrakeKey = "Xbox Brake";
                BoostKey = "Xbox Boost";
            }

            else //if there are no gamepads connected
            {
                ThrustAxis = "Thrust";
                SteeringAxis = "Steering";
                StrafeAxis = "Strafe";
                BrakeKey = "Brake";
                BoostKey = "Boost";
            }
          
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
            if (playstation)
            {
                Thrust = ((Input.GetAxis(ThrustAxis) +1) + (Input.GetAxis(playstationThrustFix)-1))/2;
            }
            else
            {
                Thrust = Input.GetAxis(ThrustAxis);
           
            }
            Steering = Input.GetAxis(SteeringAxis);
            Strafe = Input.GetAxis(StrafeAxis);
            IsBraking = Input.GetButton(BrakeKey);
            IsBoosting = Input.GetButton(BoostKey);




        }
	}
}
