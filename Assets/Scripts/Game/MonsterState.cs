using UnityEngine;

namespace Game
{
	public enum EnemyState { Guard, Patrol, Chase, Dead }

	public class MonsterState
	{
		public MonsterState(float initSpeed, Vector3 initPosition, Quaternion initRotation)
		{
			InitSpeed = initSpeed;
			InitPosition = initPosition;
			InitRotation = initRotation;
		}

		public float InitSpeed { get; set; }
		public Vector3 InitPosition { get; set; }
		public Quaternion InitRotation { get; set; }
		public bool IsChase { get; set; }
		public bool IsDead { get; set; }
		public bool IsFollow { get; set; }
		public bool IsWalk { get; set; }
	}
}