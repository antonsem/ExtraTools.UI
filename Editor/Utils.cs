using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
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
				ComponentUtility.MoveComponentUp(component);
			}
		}

		public static VisualElement AddButton(SerializedObject serializedObject, UnityEditor.Editor editor,
			string buttonText, Action clickEvent)
		{
			VisualElement inspector = new();

			InspectorElement.FillDefaultInspector(inspector, serializedObject, editor);

			Button button = new(() =>
			{
				int undoGroup = Undo.GetCurrentGroup();
				Undo.SetCurrentGroupName(buttonText);
				clickEvent();
				Undo.CollapseUndoOperations(undoGroup);
			})
			{
				text = buttonText,
			};

			inspector.Add(button);

			return inspector;
		}

		public static void AssignField<T>(T type, string fieldName, object value)
		{
			FieldInfo panelsField = typeof(T).GetField(fieldName,
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			if (panelsField == null)
			{
				return;
			}

			// Reflection bypasses serialization, so Undo, prefab overrides and saving have to be told about the change.
			Object target = type as Object;

			if (target)
			{
				Undo.RecordObject(target, "Assign " + fieldName);
			}

			panelsField.SetValue(type, value);

			if (target)
			{
				PrefabUtility.RecordPrefabInstancePropertyModifications(target);
				EditorUtility.SetDirty(target);
			}
		}

		public static TValue GetField<T, TValue>(T type, string fieldName)
		{
			FieldInfo field = typeof(T).GetField(fieldName,
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			return field != null ? (TValue)field.GetValue(type) : default;
		}

		public static TView GetOrAddView<TPresenter, TView>(TPresenter presenter, string fieldName)
			where TPresenter : Component
			where TView : Component
		{
			TView view = GetField<TPresenter, TView>(presenter, fieldName);

			if (view || (view = presenter.GetComponent<TView>()) || TryAddType(presenter, out view))
			{
				return view;
			}

			return null;
		}

		public static bool TryAddType<T>(Component neighbour, out T component) where T : Component
		{
			string assemblyName = neighbour.GetType().Assembly.FullName.Split(',')[0];
			string typeToFind = $"{neighbour.GetType().FullName}UI,{assemblyName}";
			Type type = Type.GetType(typeToFind);
			if (type != null && type.IsSubclassOf(typeof(T)))
			{
				component = Undo.AddComponent(neighbour.gameObject, type) as T;
				return true;
			}

			component = null;
			return false;
		}
	}
}
