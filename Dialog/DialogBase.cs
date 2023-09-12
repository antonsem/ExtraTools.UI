using System.Threading.Tasks;
using ExtraTools.UI.Base;
using UnityEngine;

namespace ExtraTools.UI.Dialog
{
	public abstract class DialogBase : MonoBehaviour
	{
		[SerializeField] protected DialogUIBase dialogUI;

		protected UIManagerBase UIManager;

		#region Unity Methods

		private void OnEnable()
		{
			dialogUI.OnClicked += OnClicked;
		}

		private void OnDisable()
		{
			dialogUI.OnClicked -= OnClicked;
		}

		#endregion

		internal void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
		}

		public virtual void Show(string message, DialogAnswer[] answers = null, bool hideOtherDialogs = false)
		{
			dialogUI.Setup(message, answers);
			UIManager.ShowDialog(this, hideOtherDialogs);
		}

		protected internal virtual async Task ShowAsync()
		{
			await dialogUI.ShowAsync();
		}

		protected internal virtual async Task HideAsync()
		{
			await dialogUI.HideAsync();
		}

		protected virtual void OnClicked()
		{
			UIManager.HideDialog(this);
		}
	}
}