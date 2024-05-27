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
			PanelUIBase panelUIBase = panelBase.GetComponent<PanelUIBase>();

			if (panelUIBase || Utils.TryAddType(panelBase, out panelUIBase))
			{
				Canvas canvas = panelUIBase.GetComponent<Canvas>();

				Utils.AssignFiled(panelUIBase, "_canvas", canvas);

				Utils.MoveUp(panelUIBase);
			}

			Utils.AssignFiled(panelBase, "_panelUI", panelUIBase);
			Utils.MoveUp(panelBase);
			EditorUtility.SetDirty(panelBase.gameObject);
		}
	}
}