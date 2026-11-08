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
			WidgetUIBase widgetUIBase = widgetBase.GetComponent<WidgetUIBase>();

			if (widgetUIBase || Utils.TryAddType(widgetBase, out widgetUIBase))
			{
				Canvas canvas = widgetUIBase.GetComponent<Canvas>();
				TMP_Text text = widgetUIBase.GetComponentInChildren<TMP_Text>();
				Button button = widgetUIBase.GetComponentInChildren<Button>();

				Utils.AssignField(widgetUIBase, "_canvas", canvas);
				Utils.AssignField(widgetUIBase, "_text", text);
				Utils.AssignField(widgetUIBase, "_button", button);

				Utils.MoveUp(widgetUIBase);
			}

			Utils.AssignField(widgetBase, "_widgetUI", widgetUIBase);
			Utils.MoveUp(widgetBase);
			EditorUtility.SetDirty(widgetBase.gameObject);
		}
	}
}
