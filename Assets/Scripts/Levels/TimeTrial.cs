using UnityEngine;

namespace Assets.Scripts.Levels
{
	public class TimeTrial : MonoBehaviour
	{
		[Header("Times")]
		public float Timer;
		public float Best = float.MaxValue;

		[Header("Laps")]
		public int Lap;
		public int MaxLaps;

	    public Collider Sensor; //sensor built in to the ship

		// Start is called before the first frame update
		void Start()
		{
		    Sensor = GetComponent<Collider>();
			Timer = 0.0f;
		    Lap = 0;
		}

		// Update is called once per frame
		void Update()
		{
			Timer += Time.deltaTime;
		}

	    void OnTriggerExit(Collider other) //code that is run when the player goes through the finish line
	    {

	        //if the player is just starting the race, only increase the lap number
	        if (Lap == 0)
	        {
	            Lap++;
	            return;
	        }

	        //compare the best lap time for the player with the current time
	        if (Timer < Best)
	        {
	            Best = Timer;
	        }

	        //iterate the lap number
	        Lap++;

            //reset the timer for the lap
            Timer = 0.0f;

            //check to see if the race is over
            if (Lap > MaxLaps)
	        {
	            //end the race
                //load the leaderboard file
                //add the player's score to the leaderboard
                //sort the leaderboard in order of fastest to slowest lap time
                //remove the slowest lap time from the leaderboard
                //save the leaderboard
	        }

	    }
	}
}
