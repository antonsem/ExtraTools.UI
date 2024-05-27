using ExtraTools.UI.Base;
using ExtraTools.UI.Dialog;
using ExtraTools.UI.Screen;
using ExtraTools.UI.Widget;
using UnityEditor;
using UnityEngine.UIElements;

namespace ExtraTools.UI.Editor
{
	[CustomEditor(typeof(UIManagerBase), true)]
	public class UIManagerBaseEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return Utils.AddButton(serializedObject, this, "Set UI", SetUI);
		}

		private void SetUI()
		{
			UIManagerBase uiManagerBase = (UIManagerBase)target;
			Utils.MoveUp(uiManagerBase);

			ScreenBase[] screens = uiManagerBase.GetComponentsInChildren<ScreenBase>();

			foreach (ScreenBase screen in screens)
			{
				ScreenBaseEditor.SetScreen(screen);
			}

			DialogBase[] dialogs = uiManagerBase.GetComponentsInChildren<DialogBase>();

			foreach (DialogBase dialog in dialogs)
			{
				DialogBaseEditor.SetDialog(dialog);
			}

			WidgetBase[] widgets = uiManagerBase.GetComponentsInChildren<WidgetBase>();

			foreach (WidgetBase widget in widgets)
			{
				WidgetBaseEditor.SetWidget(widget);
			}

			EditorUtility.SetDirty(uiManagerBase.gameObject);
		}
	}
}