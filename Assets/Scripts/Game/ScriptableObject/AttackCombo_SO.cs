using UnityEngine;

namespace Game.SO
{
	[CreateAssetMenu(fileName = "AttackCombo", menuName = "SO/AttackCombo")]
	public class AttackCombo_SO : ScriptableObject
	{
		public AnimatorOverrideController AnimatorOV;
		public int MinDamage;
		public int MaxDamage;
		public float AttackRange;
	}
}