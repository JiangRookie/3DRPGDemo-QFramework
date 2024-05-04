using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "AttackData", menuName = "SO/CreateAttackData")]
	public class AttackData_SO : ScriptableObject
	{
		public float AttackRange;
		public float SkillRange;
		public float CoolDown;
		public int MinDamage;
		public int MaxDamage;
		public float CriticalHitBonusPercentage;
		public float CriticalHitRate;
	}
}