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
			ScreenUIBase screenUIBase = screenBase.GetComponent<ScreenUIBase>();

			if (screenUIBase || Utils.TryAddType(screenBase, out screenUIBase))
			{
				Canvas canvas = screenUIBase.GetComponent<Canvas>();

				Utils.AssignField(screenUIBase, "_canvas", canvas);

				Utils.MoveUp(screenUIBase);
			}

			Utils.AssignField(screenBase, "_screenUI", screenUIBase);

			PanelBase[] panels = screenBase.GetComponentsInChildren<PanelBase>();
			Utils.AssignField(screenBase, "_panels", panels);

			if (panels.Length > 0)
			{
				PanelBase[] defaultPanels = { panels[0] };
				Utils.AssignField(screenBase, "_defaultPanels", defaultPanels);
			}

			foreach (PanelBase panel in panels)
			{
				PanelBaseEditor.SetPanel(panel);
			}

			Utils.MoveUp(screenBase);
			EditorUtility.SetDirty(screenBase.gameObject);
		}
	}
}
