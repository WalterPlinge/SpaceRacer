using UnityEngine;

namespace Assets.Scripts.Ship
{
	public class ShipRoll : MonoBehaviour
	{
		public float MaxRollAngle;
		public Transform Mesh;
		public PlayerInput Input;

		void Start()
		{

		}
		
		void FixedUpdate()
		{
			// Calculate roll angle
			float angle = MaxRollAngle * -Input.Steering;

			// Use ShipMeshFix to deal with broken model transform
			Quaternion roll = Quaternion.Euler(0.0f, 0.0f, angle);

			// Apply roll to mesh (Cosmetic)
			Mesh.localRotation =
				Quaternion.Slerp(
					Mesh.localRotation,
					roll,
					Time.deltaTime * 10.0f);
		}
	}
}
