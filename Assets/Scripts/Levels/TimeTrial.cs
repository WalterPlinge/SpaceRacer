using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.UI;

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

        public PlayerData playerData;
        public List<PlayerData> leaderboard = new List<PlayerData>();

        static void SaveCharacter(PlayerData data, int characterSlot)
        {
            PlayerPrefs.SetString("playerName_CharacterSlot" + characterSlot, data.playerName);
            PlayerPrefs.SetFloat("time_CharacterSlot" + characterSlot, data.playerTime);
            PlayerPrefs.Save();
        }

        static PlayerData LoadCharacter(int characterSlot)
        {
            PlayerData loadedCharacter = new PlayerData();
            loadedCharacter.playerName = PlayerPrefs.GetString("playerName_CharacterSlot" + characterSlot);
            loadedCharacter.playerTime = PlayerPrefs.GetFloat("time_CharacterSlot" + characterSlot);
            return loadedCharacter;
        }

        // Start is called before the first frame update
        void Start()
		{
            //set the variables for the game
		    Sensor = GetComponent<Collider>();
			Timer = 0.0f;
		    Lap = 0;

            //load the leaderboard file
            for (int i = 1; i <= 10; i++)
            {
                if (LoadCharacter(i) != null)
                {
                    leaderboard.Add(LoadCharacter(i));
                }
            }
            //organise the players based on laptime
            
        }

		// Update is called once per frame
		void Update()
		{
			if (Lap <= 0) return;

			Timer += Time.deltaTime;
			GameInfo.Instance.Time = Timer;
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

                //add the player's score to the leaderboard
                playerData.playerTime = Best;
                playerData.playerName = "Test";
                leaderboard.Add(playerData);
                
                //sort the leaderboard in order of fastest to slowest lap time
                leaderboard.Sort((x, y) => x.playerTime.CompareTo(y.playerTime));
                
                //remove the slowest lap time from the leaderboard
                if (leaderboard.Count > 10)
                leaderboard.RemoveAt(11);
              
                //save the leaderboard
               foreach(PlayerData p in leaderboard)
                {
                    SaveCharacter(p, leaderboard.IndexOf(p));
                    if (p.playerTime != 0)
                    {
                        print(p.playerName);
                        print(p.playerTime);
                    }
                }

               
	        }

	    }
	}
}
