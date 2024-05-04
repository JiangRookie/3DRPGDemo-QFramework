using UnityEngine;

namespace Game
{
	public class DamageCalculator
	{
		public static void TakeDamage(CharacterData attacker, CharacterData defender)
		{
			int damage = Mathf.Max(GetDamage(attacker) - defender.CurDefense, 1);
			defender.CurHealth = Mathf.Max(defender.CurHealth - damage, 0);
		}

		private static int GetDamage(CharacterData attacker)
		{
			float damage = Random.Range(attacker.MinDamage, attacker.MaxDamage + 1);
			if (attacker.IsCritical)
			{
				damage *= attacker.CriticalHitBonusPercentage;
				Debug.Log($"Critical Hit! Damage: {damage}");
			}
			else
			{
				Debug.Log($"Normal Hit! Damage: {damage}");
			}

			return (int)damage;
		}
	}
}