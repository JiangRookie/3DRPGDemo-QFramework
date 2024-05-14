using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game
{
	public class Tools
	{
		// [MenuItem("Tools/创建攻击数据")]
		public static void SetupAttackData(string writePath, string readPath)
		{
			// 先检查有没有这个文件夹：Assets/ScriptableObject/AttackData
			if (!AssetDatabase.IsValidFolder($"{writePath}/AttackData"))
			{
				AssetDatabase.CreateFolder(writePath, "AttackData");
			}

			// 读取CSV文件的数据
			TextAsset dataFile = AssetDatabase.LoadAssetAtPath<TextAsset>($"{readPath}/AttackData.csv");
			string[] textInLines = dataFile.text.Split('\n');
			string[] headers = textInLines[0].Split(',').Select(h => h.Trim()).ToArray();

			foreach (MonsterName monsterName in Enum.GetValues(typeof(MonsterName)))
			{
				string assetPath = $"{writePath}/AttackData/{monsterName}AttackBaseData.asset";
				AttackData_SO attackData = AssetDatabase.LoadAssetAtPath<AttackData_SO>(assetPath);

				// 如果资源文件不存在，则创建一个新的资源文件
				if (attackData == null)
				{
					attackData = ScriptableObject.CreateInstance<AttackData_SO>();
					AssetDatabase.CreateAsset(attackData, assetPath);
				}

				// 找到对应的索引
				int index = Array.IndexOf(headers, monsterName.ToString().Trim());
				if (index == -1) continue;

				// 根据索引来设置AttackData_SO实例的属性值
				for (int i = 1; i < textInLines.Length; i++)
				{
					string[] values = textInLines[i].Split(',');
					switch (values[0].Trim())
					{
						case "AttackRange":
							attackData.AttackRange = float.Parse(values[index]);
							break;
						case "SkillRange":
							attackData.SkillRange = float.Parse(values[index]);
							break;
						case "CoolDown":
							attackData.CoolDown = float.Parse(values[index]);
							break;
						case "MinDamage":
							attackData.MinDamage = int.Parse(values[index]);
							break;
						case "MaxDamage":
							attackData.MaxDamage = int.Parse(values[index]);
							break;
						case "CriticalHitBonusPercentage":
							attackData.CriticalHitBonusPercentage = float.Parse(values[index]);
							break;
						case "CriticalHitRate":
							attackData.CriticalHitRate = float.Parse(values[index]);
							break;
					}
				}

				// 保存资源文件的更改
				EditorUtility.SetDirty(attackData);
			}

			// 保存所有的更改
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		// [MenuItem("Tools/创建角色基础数据")]
		public static void SetupCharacterCommonData(string writePath, string readPath)
		{
			// 先检查有没有这个文件夹：Assets/ScriptableObject/CharacterBaseData/
			if (!AssetDatabase.IsValidFolder($"{writePath}/CharacterBaseData"))
			{
				AssetDatabase.CreateFolder(writePath, "CharacterBaseData");
			}

			// 读取CSV文件的数据
			TextAsset dataFile = AssetDatabase.LoadAssetAtPath<TextAsset>($"{readPath}/BaseData.csv");
			string[] textInLines = dataFile.text.Split('\n');
			string[] headers = textInLines[0].Split(',').Select(h => h.Trim()).ToArray();

			foreach (MonsterName monsterName in Enum.GetValues(typeof(MonsterName)))
			{
				string assetPath = $"{writePath}/CharacterBaseData/{monsterName}BaseData.asset";
				CharacterBaseData_SO characterData = AssetDatabase.LoadAssetAtPath<CharacterBaseData_SO>(assetPath);

				// 如果资源文件不存在，则创建一个新的资源文件
				if (characterData == null)
				{
					characterData = ScriptableObject.CreateInstance<CharacterBaseData_SO>();
					AssetDatabase.CreateAsset(characterData, assetPath);
				}

				// 找到对应的索引
				int index = Array.IndexOf(headers, monsterName.ToString().Trim());
				if (index == -1) continue;

				// 根据索引来设置CharacterCommonData_SO实例的属性值
				for (int i = 1; i < textInLines.Length; i++)
				{
					string[] values = textInLines[i].Split(',');
					switch (values[0].Trim())
					{
						case "MaxHealth":
							characterData.MaxHealth = int.Parse(values[index]);
							break;
						case "CurHealth":
							characterData.CurHealth = int.Parse(values[index]);
							break;
						case "BaseDefense":
							characterData.BaseDefense = int.Parse(values[index]);
							break;
						case "CurDefense":
							characterData.CurDefense = int.Parse(values[index]);
							break;
						case "Exp":
							characterData.Exp = int.Parse(values[index]);
							break;
					}
				}

				// 保存资源文件的更改
				EditorUtility.SetDirty(characterData);
			}

			// 保存所有的更改
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}