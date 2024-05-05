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
		}

		public static void TakeDamage(int damage, CharacterData target, Action criticalAction = null)
		{
			int realDamage = Mathf.Max(damage - target.CurDefense, 1);
			target.CurHealth = Mathf.Max(target.CurHealth - realDamage, 0);
			target.OnHealthChanged.Trigger(target.CurHealth, target.MaxHealth);
		}

		public static void TakeHurt(int damage)
		{
			int realDamage = Mathf.Max(damage - CurDefense.Value, 1);
			CurHealth.Value = Mathf.Max(CurHealth.Value - realDamage, 0);
		}
	}
}