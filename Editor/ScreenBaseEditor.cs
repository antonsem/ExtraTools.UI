using System;
using ExtraTools.UI.Panel;
using ExtraTools.UI.Screen;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtraTools.UI.Editor
{
	[CustomEditor(typeof(ScreenBase), true)]
	public class ScreenBaseEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return Utils.AddButton(serializedObject, this, "Set Screen", SetScreen);
		}

		private void SetScreen()
		{
			SetScreen((ScreenBase)target);
		}

		public static void SetScreen(ScreenBase screenBase)
		{
			ScreenUIBase screenUIBase = Utils.GetOrAddView<ScreenBase, ScreenUIBase>(screenBase, "_screenUI");

			if (screenUIBase)
			{
				Canvas canvas = screenUIBase.GetComponent<Canvas>();

				Utils.AssignField(screenUIBase, "_canvas", canvas);

				Utils.MoveUp(screenUIBase);
			}

			Utils.AssignField(screenBase, "_screenUI", screenUIBase);

			PanelBase[] panels = screenBase.GetComponentsInChildren<PanelBase>();
			Utils.AssignField(screenBase, "_panels", panels);

			// Keep the configured defaults, minus panels that were deleted or no longer belong to this screen.
			PanelBase[] defaultPanels = Utils.GetField<ScreenBase, PanelBase[]>(screenBase, "_defaultPanels")
				?? Array.Empty<PanelBase>();
			defaultPanels = Array.FindAll(defaultPanels, panel => panel && Array.IndexOf(panels, panel) >= 0);

			if (defaultPanels.Length == 0 && panels.Length > 0)
			{
				defaultPanels = new[] { panels[0] };
			}

			Utils.AssignField(screenBase, "_defaultPanels", defaultPanels);

			foreach (PanelBase panel in panels)
			{
				PanelBaseEditor.SetPanel(panel);
			}

			Utils.MoveUp(screenBase);
			EditorUtility.SetDirty(screenBase.gameObject);
		}
	}
}
