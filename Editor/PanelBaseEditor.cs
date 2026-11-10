using ExtraTools.UI.Panel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtraTools.UI.Editor
{
	[CustomEditor(typeof(PanelBase), true)]
	public class PanelBaseEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return Utils.AddButton(serializedObject, this, "Set Panel", SetPanel);
		}

		private void SetPanel()
		{
			SetPanel((PanelBase)target);
		}

		internal static void SetPanel(PanelBase panelBase)
		{
			PanelUIBase panelUIBase = Utils.GetOrAddView<PanelBase, PanelUIBase>(panelBase, "PanelUI");

			if (panelUIBase)
			{
				Canvas canvas = panelUIBase.GetComponent<Canvas>();

				Utils.AssignField(panelUIBase, "Canvas", canvas);

				Utils.MoveUp(panelUIBase);
			}

			Utils.AssignField(panelBase, "PanelUI", panelUIBase);
			Utils.MoveUp(panelBase);
			EditorUtility.SetDirty(panelBase.gameObject);
		}
	}
}
