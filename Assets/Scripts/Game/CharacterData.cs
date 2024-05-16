using Game.SO;
using QFramework;
using UnityEngine;

namespace Game
{
	public class CharacterData : MonoBehaviour
	{
		public MonsterName MonsterName;
		public CharacterBaseData_SO CharacterBaseData;
		public CharacterBaseData_SO TemplateCharacterData;
		public AttackData_SO AttackData;
		public EasyEvent<int, int> OnHealthChanged = new EasyEvent<int, int>();
		public bool IsCritical { get; set; }

		#region CharacterBaseData

		private void Awake()
		{
			if (TemplateCharacterData)
			{
				CharacterBaseData = Instantiate(TemplateCharacterData);
			}
		}

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
			get => AttackData ? AttackData.AttackRange : 0;
			set
			{
				if (AttackData) AttackData.AttackRange = value;
			}
		}

		public float SkillRange
		{
			get => AttackData ? AttackData.SkillRange : 0;
			set
			{
				if (AttackData) AttackData.SkillRange = value;
			}
		}

		public float CoolDown
		{
			get => AttackData ? AttackData.CoolDown : 0;
			set
			{
				if (AttackData) AttackData.CoolDown = value;
			}
		}

		public int MinDamage
		{
			get => AttackData ? AttackData.MinDamage : 0;
			set
			{
				if (AttackData) AttackData.MinDamage = value;
			}
		}

		public int MaxDamage
		{
			get => AttackData ? AttackData.MaxDamage : 0;
			set
			{
				if (AttackData) AttackData.MaxDamage = value;
			}
		}

		public float CriticalHitBonusPercentage
		{
			get => AttackData ? AttackData.CriticalHitBonusPercentage : 0;
			set
			{
				if (AttackData) AttackData.CriticalHitBonusPercentage = value;
			}
		}

		public float CriticalHitRate
		{
			get => AttackData ? AttackData.CriticalHitRate : 0;
			set
			{
				if (AttackData) AttackData.CriticalHitRate = value;
			}
		}

		public int Exp
		{
			get => CharacterBaseData ? CharacterBaseData.Exp : 0;
			set
			{
				if (CharacterBaseData) CharacterBaseData.Exp = value;
			}
		}

		#endregion
	}
}