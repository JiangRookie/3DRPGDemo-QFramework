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

		public static void Load()
		{
			MaxHealth.Value = PlayerPrefs.GetInt("maxHealth", 100);
			CurHealth.Value = PlayerPrefs.GetInt("curHealth", 100);
			BaseDefense.Value = PlayerPrefs.GetInt("baseDefense", 2);
			CurDefense.Value = PlayerPrefs.GetInt("curDefense", 2);
			AttackRange.Value = PlayerPrefs.GetFloat("attackRange", 2);
			SkillRange.Value = PlayerPrefs.GetFloat("skillRange", 0);
			CoolDown.Value = PlayerPrefs.GetFloat("coolDown", 0.7f);
			MinDamage.Value = PlayerPrefs.GetInt("minDamage", 4);
			MaxDamage.Value = PlayerPrefs.GetInt("maxDamage", 6);
			CriticalHitBonusPercentage.Value = PlayerPrefs.GetFloat("criticalHitBonusPercentage", 2f);
			CriticalHitRate.Value = PlayerPrefs.GetFloat("criticalHitRate", 0.2f);
			CurLevel.Value = PlayerPrefs.GetInt("curLevel", 1);
			MaxLevel.Value = PlayerPrefs.GetInt("maxLevel", 30);
			ExpToNextLevel.Value = PlayerPrefs.GetInt("expToNextLevel", 30);
			CurExp.Value = PlayerPrefs.GetInt("curExp", 0);
			LevelBuff.Value = PlayerPrefs.GetFloat("levelBuff", 0.1f);
		}

		public static void Save()
		{
			MaxHealth.Register(maxHealth => PlayerPrefs.SetInt("maxHealth", maxHealth));
			CurHealth.Register(curHealth => PlayerPrefs.SetInt("curHealth", curHealth));
			BaseDefense.Register(baseDefense => PlayerPrefs.SetInt("baseDefense", baseDefense));
			CurDefense.Register(curDefense => PlayerPrefs.SetInt("curDefense", curDefense));
			AttackRange.Register(attackRange => PlayerPrefs.SetFloat("attackRange", attackRange));
			SkillRange.Register(skillRange => PlayerPrefs.SetFloat("skillRange", skillRange));
			CoolDown.Register(coolDown => PlayerPrefs.SetFloat("coolDown", coolDown));
			MinDamage.Register(minDamage => PlayerPrefs.SetInt("minDamage", minDamage));
			MaxDamage.Register(maxDamage => PlayerPrefs.SetInt("maxDamage", maxDamage));
			CriticalHitBonusPercentage.Register(criticalHitBonusPercentage => PlayerPrefs.SetFloat("criticalHitBonusPercentage", criticalHitBonusPercentage));
			CriticalHitRate.Register(criticalHitRate => PlayerPrefs.SetFloat("criticalHitRate", criticalHitRate));
			CurLevel.Register(curLevel => PlayerPrefs.SetInt("curLevel", curLevel));
			MaxLevel.Register(maxLevel => PlayerPrefs.SetInt("maxLevel", maxLevel));
			ExpToNextLevel.Register(expToNextLevel => PlayerPrefs.SetInt("expToNextLevel", expToNextLevel));
			CurExp.Register(curExp => PlayerPrefs.SetInt("curExp", curExp));
			LevelBuff.Register(levelBuff => PlayerPrefs.SetFloat("levelBuff", levelBuff));
		}

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
		}
	}
}