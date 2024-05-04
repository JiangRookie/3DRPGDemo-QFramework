﻿using UnityEngine;

namespace Game
{
	public class CharacterData : MonoBehaviour
	{
		public CharacterCommonData_SO CharacterBaseData;
		public AttackData_SO CharacterAttackData;
		[HideInInspector] public bool IsCritical;

		#region CharacterBaseData

		public int MaxHealth
		{
			get => CharacterBaseData ? CharacterBaseData.MaxHealth : 0;
			set
			{
				if (CharacterBaseData) CharacterBaseData.MaxHealth = value;
			}
		}

		public int CurHealth
		{
			get => CharacterBaseData ? CharacterBaseData.CurHealth : 0;
			set
			{
				if (CharacterBaseData != null) CharacterBaseData.CurHealth = value;
			}
		}

		public int BaseDefense
		{
			get => CharacterBaseData ? CharacterBaseData.BaseDefense : 0;
			set
			{
				if (CharacterBaseData) CharacterBaseData.BaseDefense = value;
			}
		}

		public int CurDefense
		{
			get => CharacterBaseData ? CharacterBaseData.CurDefense : 0;
			set
			{
				if (CharacterBaseData) CharacterBaseData.CurDefense = value;
			}
		}

		#endregion

		#region CharacterAttackData

		public float AttackRange
		{
			get => CharacterAttackData ? CharacterAttackData.AttackRange : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.AttackRange = value;
			}
		}

		public float SkillRange
		{
			get => CharacterAttackData ? CharacterAttackData.SkillRange : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.SkillRange = value;
			}
		}

		public float CoolDown
		{
			get => CharacterAttackData ? CharacterAttackData.CoolDown : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.CoolDown = value;
			}
		}

		public int MinDamage
		{
			get => CharacterAttackData ? CharacterAttackData.MinDamage : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.MinDamage = value;
			}
		}

		public int MaxDamage
		{
			get => CharacterAttackData ? CharacterAttackData.MaxDamage : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.MaxDamage = value;
			}
		}

		public float CriticalHitBonusPercentage
		{
			get => CharacterAttackData ? CharacterAttackData.CriticalHitBonusPercentage : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.CriticalHitBonusPercentage = value;
			}
		}

		public float CriticalHitRate
		{
			get => CharacterAttackData ? CharacterAttackData.CriticalHitRate : 0;
			set
			{
				if (CharacterAttackData) CharacterAttackData.CriticalHitRate = value;
			}
		}

		#endregion
	}
}