using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "AttackCombo", menuName = "SO/AttackCombo")]
	public class AttackCombo_SO : ScriptableObject
	{
		public AnimatorOverrideController AnimatorOV;
		public int Damage;
	}
}