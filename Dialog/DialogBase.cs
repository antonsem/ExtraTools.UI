using System.Threading.Tasks;
using ExtraTools.UI.Base;
using UnityEngine;

namespace ExtraTools.UI.Dialog
{
	public abstract class DialogBase : MonoBehaviour
	{
		[SerializeField] protected DialogUIBase _dialogUI;

		protected UIManagerBase UIManager;

		#region Unity Methods

		private void OnEnable()
		{
			_dialogUI.OnClicked += OnClicked;
		}

		private void OnDisable()
		{
			_dialogUI.OnClicked -= OnClicked;
		}

		#endregion

		internal void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
		}

		public virtual void Show(string message, DialogAnswer[] answers = null, bool hideOtherDialogs = false)
		{
			_dialogUI.Setup(message, answers);
			UIManager.ShowDialog(this, hideOtherDialogs);
		}

		protected internal virtual async Task ShowAsync()
		{
			await _dialogUI.ShowAsync();
		}

		protected internal virtual async Task HideAsync()
		{
			await _dialogUI.HideAsync();
		}

		protected virtual void OnClicked()
		{
			UIManager.HideDialog(this);
		}

#if UNITY_EDITOR
		[ContextMenu("Set Dialog")]
		internal void SetDialog()
		{
			_dialogUI = GetComponent<DialogUIBase>();

			if (_dialogUI)
			{
				_dialogUI.SetDialogUI();
			}
		}
#endif
	}
}