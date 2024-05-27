using ExtraTools.UI.Dialog;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtraTools.UI.Editor
{
	[CustomEditor(typeof(DialogBase), true)]
	public class DialogBaseEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			return Utils.AddButton(serializedObject, this, "Set Dialog", SetDialog);
		}

		private void SetDialog()
		{
			SetDialog((DialogBase)target);
		}

		public static void SetDialog(DialogBase dialogBase)
		{
			DialogUIBase dialogUIBase = dialogBase.GetComponent<DialogUIBase>();

			if (dialogUIBase || Utils.TryAddType(dialogBase, out dialogUIBase))
			{
				Canvas canvas = dialogUIBase.GetComponent<Canvas>();

				Utils.AssignFiled(dialogUIBase, "_canvas", canvas);

				Utils.InvokeMethod(dialogUIBase, "SetDialogUI");
				Utils.MoveUp(dialogUIBase);
			}

			Utils.AssignFiled(dialogBase, "_dialogUI", dialogUIBase);
			Utils.MoveUp(dialogBase);
			EditorUtility.SetDirty(dialogBase.gameObject);
		}
	}
}