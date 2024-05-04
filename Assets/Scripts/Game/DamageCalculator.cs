using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
	public class DamageCalculator
	{
		public static bool CalculateIsCritical(CharacterData attacker)
		{
			return Random.value <= attacker.CriticalHitRate;
		}

		public static void TakeDamage(CharacterData attacker, CharacterData defender, Action criticalAction)
		{
			int damage = Mathf.Max(GetDamage(attacker) - defender.CurDefense, 1);
			defender.CurHealth = Mathf.Max(defender.CurHealth - damage, 0);
			if (attacker.IsCritical)
			{
				criticalAction?.Invoke();
			}
		}

		private static int GetDamage(CharacterData attacker)
		{
			float damage = Random.Range(attacker.MinDamage, attacker.MaxDamage + 1);
			attacker.IsCritical = Random.value <= attacker.CriticalHitRate;
			if (attacker.IsCritical)
			{
				damage *= attacker.CriticalHitBonusPercentage;
			}

			return (int)damage;
		}
	}
}