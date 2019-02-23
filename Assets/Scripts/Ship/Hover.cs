using UnityEngine;

namespace Assets.Scripts.Ship
{
	public class Hover : MonoBehaviour
	{
		public Rigidbody Ship;

		public double TargetHeight;
		public double MaxDistance;
		public double Force;

		public PidController Pid;

		// Start is called before the first frame update
		private void Start()
		{
			Ship = GetComponent<Rigidbody>();

			TargetHeight = 1;
			MaxDistance = 4;
			Force = 128;

			Pid = new PidController();
		}

		// Update is called once per frame
		private void FixedUpdate()
		{
			
		}
	}
}
