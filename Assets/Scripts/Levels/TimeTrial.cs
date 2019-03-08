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

		// Start is called before the first frame update
		void Start()
		{
			Timer = 0.0f;
		}

		// Update is called once per frame
		void Update()
		{
			Timer += Time.deltaTime;
		}
	}
}
