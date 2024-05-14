using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Tools = Game.Tools;

namespace Editor
{
	public class CharacterDataSetupTool : EditorWindow
	{
		private string _ToWriteFileFolderPath;
		private string _ToReadFileFolderPath;

		[MenuItem("Tools/创建角色基础数据")]
		private static void OpenWindow()
		{
			var window = GetWindowWithRect(typeof(CharacterDataSetupTool), new Rect(0, 0, 350, 220)) as CharacterDataSetupTool;
			if (window)
			{
				window.Show();
			}
		}

		private void OnGUI()
		{
			GUILayout.Label("拖拽生成 ScriptableObject.asset 文件的目标文件夹：");

			var dragArea1 = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
			GUI.Box(dragArea1, _ToWriteFileFolderPath);

			if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) && dragArea1.Contains(Event.current.mousePosition))
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					foreach (var draggedObject in DragAndDrop.objectReferences)
					{
						var path = AssetDatabase.GetAssetPath(draggedObject);
						if (Directory.Exists(path))
						{
							_ToWriteFileFolderPath = path;
						}
					}
				}

				Event.current.Use();
			}

			GUILayout.Label("拖拽需要读取数据的文件夹：");
			var dragArea2 = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));

			GUI.Box(dragArea2, _ToReadFileFolderPath);

			if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) && dragArea2.Contains(Event.current.mousePosition))
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					foreach (var draggedObject in DragAndDrop.objectReferences)
					{
						var path = AssetDatabase.GetAssetPath(draggedObject);
						if (Directory.Exists(path))
						{
							_ToReadFileFolderPath = path;
						}
					}
				}

				Event.current.Use();
			}

			if (GUILayout.Button("开始生成", GUILayout.Height(50)))
			{
				if (_ToWriteFileFolderPath == string.Empty || _ToReadFileFolderPath == string.Empty)
				{
					throw new Exception("请拖拽文件夹到对应的区域");
				}
				Tools.SetupCharacterCommonData(_ToWriteFileFolderPath, _ToReadFileFolderPath);
				Tools.SetupAttackData(_ToWriteFileFolderPath, _ToReadFileFolderPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}
}