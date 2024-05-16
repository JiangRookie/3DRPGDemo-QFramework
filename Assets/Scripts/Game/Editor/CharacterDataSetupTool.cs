#if UNITY_EDITOR
using System;
using System.IO;
using Game;
using UnityEditor;
using UnityEngine;

namespace OhJiang.Editor
{
	public class CharacterDataSetupTool : EditorWindow
	{
		private string _ToWriteFileFolderPath = "Assets/ScriptableObject";
		private string _ToReadFileFolderPath = "Assets/Res/Data";

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
			HandleDragAndDrop(ref _ToWriteFileFolderPath);

			GUILayout.Label("拖拽需要读取数据的文件夹：");
			HandleDragAndDrop(ref _ToReadFileFolderPath);

			if (GUILayout.Button("开始生成", GUILayout.Height(50)))
			{
				if (_ToWriteFileFolderPath == string.Empty || _ToReadFileFolderPath == string.Empty)
				{
					throw new Exception("请拖拽文件夹到对应的区域");
				}
				GameDataSetuper.SetupCharacterBaseData(_ToWriteFileFolderPath, _ToReadFileFolderPath);
				GameDataSetuper.SetupAttackData(_ToWriteFileFolderPath, _ToReadFileFolderPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		private void HandleDragAndDrop(ref string folderPath)
		{
			var dragArea = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
			GUI.Box(dragArea, folderPath);

			if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) && dragArea.Contains(Event.current.mousePosition))
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
							folderPath = path;
						}
					}
				}

				Event.current.Use();
			}
		}
	}
}
#endif