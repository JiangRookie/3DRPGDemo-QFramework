using System.Reflection;
using OhJiang.Attributes;
using UnityEditor;
using UnityEngine;

namespace OhJiang.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(MonoBehaviour), true)]
	public class InspectorButtonMonoEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			MonoBehaviour script = (MonoBehaviour)target;
			var methodInfos = script.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var methodInfo in methodInfos)
			{
				var attributes = methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true);
				if (attributes.Length > 0)
				{
					if (GUILayout.Button(methodInfo.Name))
					{
						methodInfo.Invoke(script, null);
					}
				}
			}
		}
	}

	[CanEditMultipleObjects]
	[CustomEditor(typeof(ScriptableObject), true)]
	public class InspectorButtonSOEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			ScriptableObject script = (ScriptableObject)target;
			var methodInfos = script.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var methodInfo in methodInfos)
			{
				var attributes = methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true);
				if (attributes.Length > 0)
				{
					if (GUILayout.Button(methodInfo.Name))
					{
						methodInfo.Invoke(script, null);
					}
				}
			}
		}
	}
}