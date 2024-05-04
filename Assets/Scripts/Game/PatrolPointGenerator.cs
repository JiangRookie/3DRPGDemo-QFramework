using UnityEngine;
using UnityEngine.AI;

namespace Game
{
	public class PatrolPointGenerator
	{
		private float _PatrolRange;
		private Vector3 _InitPosition;

		public PatrolPointGenerator(float patrolRange, Vector3 initPosition)
		{
			_PatrolRange = patrolRange;
			_InitPosition = initPosition;
		}

		public Vector3 GeneratePatrolPoint()
		{
			float randomX = Random.Range(-_PatrolRange, _PatrolRange);
			float randomZ = Random.Range(-_PatrolRange, _PatrolRange);
			var newPoint = new Vector3(_InitPosition.x + randomX, _InitPosition.y, _InitPosition.z + randomZ);
			return NavMesh.SamplePosition(newPoint, out NavMeshHit hit, _PatrolRange, 1)
				? hit.position
				: _InitPosition;
		}
	}
}