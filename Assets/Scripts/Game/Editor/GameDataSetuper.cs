#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Game.SO;
using UnityEditor;
using UnityEngine;

namespace Game
{
	public static class GameDataSetuper
	{
		public static void SetupAttackData(string writePath, string readPath)
		{
			SetupData<AttackData_SO>(writePath, readPath, "AttackData");
		}

		public static void SetupCharacterBaseData(string writePath, string readPath)
		{
			SetupData<CharacterBaseData_SO>(writePath, readPath, "CharacterBaseData");
		}

		private static void SetupData<T>(string writePath, string readPath, string folderName) where T : ScriptableObject
		{
			if (!AssetDatabase.IsValidFolder($"{writePath}/{folderName}"))
			{
				AssetDatabase.CreateFolder(writePath, folderName);
			}

			string fileName = $"{folderName}.csv";
			TextAsset dataFile = AssetDatabase.LoadAssetAtPath<TextAsset>($"{readPath}/{fileName}");
			string[] textInLines = dataFile.text.Split('\n');
			string[] headers = textInLines[0].Split(',').Select(h => h.Trim()).ToArray();

			foreach (MonsterName monsterName in Enum.GetValues(typeof(MonsterName)))
			{
				string assetPath = $"{writePath}/{folderName}/{monsterName}BaseData.asset";
				T data = AssetDatabase.LoadAssetAtPath<T>(assetPath);

				if (data == null)
				{
					data = ScriptableObject.CreateInstance<T>();
					AssetDatabase.CreateAsset(data, assetPath);
				}

				int index = Array.IndexOf(headers, monsterName.ToString().Trim());
				if (index == -1) continue;

				FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

				for (int i = 1; i < textInLines.Length; i++)
				{
					string[] values = textInLines[i].Split(',');
					string fieldName = values[0].Trim();
					FieldInfo field = fields.FirstOrDefault(f => f.Name == fieldName);
					if (field != null)
					{
						object value = Convert.ChangeType(values[index], field.FieldType);
						field.SetValue(data, value);
					}
				}

				EditorUtility.SetDirty(data);
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}
#endif