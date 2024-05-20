using QFramework;
using UnityEngine;

namespace Game
{
	public partial class RockGenerate : ViewController
	{
		private static RockGenerate s_Instance;

		private void Awake()
		{
			s_Instance = this;
		}

		public static void GenerateRock(Vector3 spawnPos, GameObject attackTarget)
		{
			s_Instance.Rock
			   .InstantiateWithParent(s_Instance.transform)
			   .Position(spawnPos)
			   .RotationIdentity()
			   .SetAttackTarget(attackTarget);
		}
	}
}