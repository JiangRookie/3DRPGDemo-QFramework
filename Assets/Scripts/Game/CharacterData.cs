using Game.SO;
using OhJiang.Attributes;
using QFramework;
using UnityEditor;
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

		[Button]
		private void SetupCharacterData()
		{
			// Get all selected GameObjects
			GameObject[] selectedObjects = Selection.gameObjects;

			// Process each selected GameObject
			foreach (GameObject selectedObject in selectedObjects)
			{
				CharacterData characterData = selectedObject.GetComponent<CharacterData>();
				if (characterData != null)
				{
					// Get all ScriptableObject assets from the "Assets/ScriptableObject" directory
					string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/ScriptableObject" });
					foreach (string guid in guids)
					{
						string assetPath = AssetDatabase.GUIDToAssetPath(guid);
						ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

						// Check if the asset's name contains the MonsterName
						if (asset.name.Contains(characterData.MonsterName.ToString()))
						{
							// Check the type of the asset and assign it to the corresponding field
							if (asset is CharacterBaseData_SO)
							{
								characterData.TemplateCharacterData = asset as CharacterBaseData_SO;
							}
							else if (asset is AttackData_SO)
							{
								characterData.AttackData = asset as AttackData_SO;
							}
						}
					}
				}
			}
		}
	}
}