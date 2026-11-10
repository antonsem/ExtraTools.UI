using ExtraTools.UI.Widget;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace ExtraTools.UI.Editor
{
	[CustomEditor(typeof(WidgetBase), true)]
	public class WidgetBaseEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return Utils.AddButton(serializedObject, this, "Set Widget", SetWidget);
		}

		private void SetWidget()
		{
			SetWidget((WidgetBase)target);
		}

		public static void SetWidget(WidgetBase widgetBase)
		{
			WidgetUIBase widgetUIBase = Utils.GetOrAddView<WidgetBase, WidgetUIBase>(widgetBase, "_widgetUI");

			if (widgetUIBase)
			{
				Canvas canvas = widgetUIBase.GetComponent<Canvas>();
				Utils.AssignField(widgetUIBase, "_canvas", canvas);

				// Text and button assigned by hand may live outside the view's hierarchy, so only fill empty fields.
				if (!Utils.GetField<WidgetUIBase, TMP_Text>(widgetUIBase, "_text"))
				{
					TMP_Text text = widgetUIBase.GetComponentInChildren<TMP_Text>();
					Utils.AssignField(widgetUIBase, "_text", text);
				}

				if (!Utils.GetField<WidgetUIBase, Button>(widgetUIBase, "_button"))
				{
					Button button = widgetUIBase.GetComponentInChildren<Button>();
					Utils.AssignField(widgetUIBase, "_button", button);
				}

				Utils.MoveUp(widgetUIBase);
			}

			Utils.AssignField(widgetBase, "_widgetUI", widgetUIBase);
			Utils.MoveUp(widgetBase);
			EditorUtility.SetDirty(widgetBase.gameObject);
		}
	}
}
