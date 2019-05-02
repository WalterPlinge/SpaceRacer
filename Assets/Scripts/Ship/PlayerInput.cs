using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ship
{
	public class PlayerInput : MonoBehaviour
	{

        private bool xbox;
        private bool playstation;

        public string playstationThrustFix;
		public string ThrustAxis;
		public string SteeringAxis;
		public string StrafeAxis;
		public string BrakeKey;
		public string BoostKey;
        public string Exit;

		[HideInInspector] public float Thrust;
		[HideInInspector] public float Strafe;
		[HideInInspector] public float Steering;
		[HideInInspector] public bool IsBraking;
		[HideInInspector] public bool IsBoosting;

		void Awake()
		{
			xbox = false;
			playstation = false;

            //find out if the player is using an xbox or playstation controller
            string[] names = Input.GetJoystickNames();
			print(names.Length);
            for (int x = 0; x < names.Length; x++)
            {
                print(names[x]);
                if (names[x].Length == 19)
                {
                   
                    playstation = true;
                    xbox = false;
                }
                if (names[x].Length == 33)
                {
                    playstation = false;
                    xbox = true;
                }
            }

            if (xbox) //if an xbox controller is connected
            {
                print("Xbox controller is connected");
                ThrustAxis = "Xbox Thrust";
                SteeringAxis = "Xbox Steering";
                StrafeAxis = "Xbox Strafe";
                BrakeKey = "Xbox Brake";
                BoostKey = "Xbox Boost";
                Exit = "Exit";
            }
            else if (playstation) // if a playstation controller is connected
            {
                print("PS4 controller is connected");
                playstationThrustFix = "Playstation L2";
                ThrustAxis = "Playstation R2";
                SteeringAxis = "Xbox Steering";
                StrafeAxis = "Xbox Strafe";
                BrakeKey = "Xbox Brake";
                BoostKey = "Xbox Boost";
                Exit = "Playstation Exit";
            }
            else //if there are no gamepads connected
            {
                print("Keyboard is connected");
                ThrustAxis = "Thrust";
                SteeringAxis = "Steering";
                StrafeAxis = "Strafe";
                BrakeKey = "Brake";
                BoostKey = "Boost";
                Exit = "Exit";
            }
          
		}

		void Update()
		{
			// When exit button is pressed, bring up the pause menu
			if (Input.GetButtonDown(Exit) && !Application.isEditor && SceneManager.GetSceneByName("EscapeMenu").isLoaded == false)
            {
				// Pause audio
	            foreach (var audioSource in FindObjectsOfType<AudioSource>())
		            audioSource.Pause();

                print("Exit Axis has been pressed");
                SceneManager.LoadSceneAsync("EscapeMenu", LoadSceneMode.Additive);
                Time.timeScale = 0;
            }




            // Get thrust value for the ship
            if (playstation)
            {
                //because of the way that playstation triggers are reported, we need to do dome math
                Thrust = ((Input.GetAxis(ThrustAxis) +1) + (Input.GetAxis(playstationThrustFix)-1))/2;
            }
            else
            {
                Thrust = Input.GetAxis(ThrustAxis);
           
            }
            
            //get the other values
            Steering = Input.GetAxis(SteeringAxis);
            Strafe = Input.GetAxis(StrafeAxis);
            IsBraking = Input.GetButton(BrakeKey);
            IsBoosting = Input.GetButton(BoostKey);




        }
	}
}
