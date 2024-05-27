using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ExtraTools.UI.Editor
{
	public static class Utils
	{
		public static void InvokeMethod(Object obj, string methodName)
		{
			MethodInfo dynMethod = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			dynMethod?.Invoke(obj, null);
		}

		public static void MoveUp(Component component)
		{
			int componentCount = component.GetComponents<Component>().Length;

			for (int i = 0; i < componentCount; i++)
			{
				UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
			}
		}

		public static VisualElement AddButton(SerializedObject serializedObject, UnityEditor.Editor editor, string buttonText, Action clickEvent)
		{
			VisualElement inspector = new();

			InspectorElement.FillDefaultInspector(inspector, serializedObject, editor);

			Button button = new(clickEvent)
			{
				text = buttonText
			};

			inspector.Add(button);

			return inspector;
		}

		public static void AssignFiled<T>(T type, string fieldName, object value)
		{
			FieldInfo panelsField = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			panelsField?.SetValue(type, value);
		}

		public static bool TryAddType<T>(Component neighbour, out T component) where T : Component
		{
			string assemblyName = neighbour.GetType().Assembly.FullName.Split(',')[0];
			string typeToFind = $"{neighbour.GetType().FullName}UI,{assemblyName}";
			Type type = Type.GetType(typeToFind);
			if (type != null && type.IsSubclassOf(typeof(T)))
			{
				component = neighbour.gameObject.AddComponent(type) as T;
				return true;
			}

			component = null;
			return false;
		}
	}
}