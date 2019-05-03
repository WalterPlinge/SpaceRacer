using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class GameHud : MonoBehaviour
	{
		public Text Timer;
		public Text Speed;
		public Text Boost;
        public Text Laps;

		// Start is called before the first frame update
		void Start()
		{

            
        }

		// Update is called once per frame
		void Update()
		{
			Timer.text = GameInfo.Instance.Time <= 0.0f
				? "00:00.000"
				: TimeSpan.FromSeconds(GameInfo.Instance.Time).ToString(@"mm\:ss\.fff");

			Speed.text = GameInfo.Instance.Speed.ToString();

			if (GameInfo.Instance.Boost < 0)
				Boost.text = "0%";
			else if (GameInfo.Instance.Boost > 100)
				Boost.text = "100%";
			else
				Boost.text = GameInfo.Instance.Boost.ToString() + "%";

            Laps.text = GameInfo.Instance.Lap.ToString() + "/" + GameInfo.Instance.maxLaps.ToString();
		}
	}
}
