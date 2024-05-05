using System;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
	public class PlayerData
	{
		public static BindableProperty<int> MaxHealth = new BindableProperty<int>(100);
		public static BindableProperty<int> CurHealth = new BindableProperty<int>(100);
		public static BindableProperty<int> BaseDefense = new BindableProperty<int>(2);
		public static BindableProperty<int> CurDefense = new BindableProperty<int>(2);

		public static BindableProperty<float> AttackRange = new BindableProperty<float>(2);
		public static BindableProperty<float> SkillRange = new BindableProperty<float>(0f);
		public static BindableProperty<float> CoolDown = new BindableProperty<float>(0.7f);
		public static BindableProperty<int> MinDamage = new BindableProperty<int>(4);
		public static BindableProperty<int> MaxDamage = new BindableProperty<int>(6);
		public static BindableProperty<float> CriticalHitBonusPercentage = new BindableProperty<float>(2f);
		public static BindableProperty<float> CriticalHitRate = new BindableProperty<float>(0.2f);
		public static BindableProperty<bool> IsCritical = new BindableProperty<bool>(false);

		public static BindableProperty<int> CurLevel = new BindableProperty<int>(1);
		public static BindableProperty<int> MaxLevel = new BindableProperty<int>(30);
		public static BindableProperty<int> ExpToNextLevel = new BindableProperty<int>(30);
		public static BindableProperty<int> CurExp = new BindableProperty<int>(0);
		public static BindableProperty<float> LevelBuff = new BindableProperty<float>(0.1f);
		private static float LevelMultiplier => 1 + (CurLevel.Value - 1) * LevelBuff.Value;

		public static void TakeDamage(CharacterData target, Action criticalAction = null)
		{
			float baseDamage = Random.Range(MinDamage.Value, MaxDamage.Value + 1);
			if (IsCritical.Value)
			{
				baseDamage *= CriticalHitBonusPercentage.Value;
				criticalAction?.Invoke();
			}
			int realDamage = Mathf.Max((int)baseDamage - target.CurDefense, 1);
			target.CurHealth = Mathf.Max(target.CurHealth - realDamage, 0);
			target.OnHealthChanged.Trigger(target.CurHealth, target.MaxHealth);
			if (target.CurHealth <= 0)
			{
				UpdateExp(target.Exp);
			}
		}

		public static void TakeDamage(int damage, CharacterData target, Action criticalAction = null)
		{
			int realDamage = Mathf.Max(damage - target.CurDefense, 1);
			target.CurHealth = Mathf.Max(target.CurHealth - realDamage, 0);
			target.OnHealthChanged.Trigger(target.CurHealth, target.MaxHealth);
			if (target.CurHealth <= 0)
			{
				UpdateExp(target.Exp);
			}
		}

		public static void TakeHurt(int damage)
		{
			int realDamage = Mathf.Max(damage - CurDefense.Value, 1);
			CurHealth.Value = Mathf.Max(CurHealth.Value - realDamage, 0);
		}

		private static void UpdateExp(int exp)
		{
			CurExp.Value += exp;
			if (CurExp.Value > ExpToNextLevel.Value)
			{
				LevelUp();
			}
		}

		private static void LevelUp()
		{
			CurLevel.Value = Mathf.Clamp(CurLevel.Value + 1, 1, MaxLevel.Value);
			ExpToNextLevel.Value += (int)(ExpToNextLevel.Value * LevelMultiplier);
			MaxHealth.Value = (int)(MaxHealth.Value * LevelMultiplier);
			CurHealth.Value = MaxHealth.Value;
			Debug.Log($"Level Up! Current Level: {CurLevel.Value.ToString()}");
		}
	}
}