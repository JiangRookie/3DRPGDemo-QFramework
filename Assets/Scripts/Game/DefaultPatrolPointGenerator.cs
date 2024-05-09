using UnityEngine;
using UnityEngine.AI;

namespace Game
{
	public interface IPatrolPointGenerator
	{
		Vector3 WayPoint { get; }
		void GeneratePatrolPoint();
	}

	public class DefaultPatrolPointGenerator : IPatrolPointGenerator
	{
		private Vector3 _InitPosition;
		private float _PatrolRange;

		public DefaultPatrolPointGenerator(float patrolRange, Vector3 initPosition)
		{
			_PatrolRange = patrolRange;
			_InitPosition = initPosition;
		}

		public Vector3 WayPoint { get; private set; }

		public void GeneratePatrolPoint()
		{
			float randomX = Random.Range(-_PatrolRange, _PatrolRange);
			float randomZ = Random.Range(-_PatrolRange, _PatrolRange);
			var newPoint = new Vector3(_InitPosition.x + randomX, _InitPosition.y, _InitPosition.z + randomZ);
			WayPoint = NavMesh.SamplePosition(newPoint, out NavMeshHit hit, _PatrolRange, 1)
				? hit.position
				: _InitPosition;
		}
	}
}