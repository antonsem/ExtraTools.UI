using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using UnityEngine;

namespace ExtraTools.UI.Dialog
{
	public abstract class DialogBase : MonoBehaviour
	{
		[SerializeField] protected DialogUIBase _dialogUI;

		protected UIManagerBase UIManager;

		#region Unity Methods

		protected virtual void OnEnable()
		{
			_dialogUI.OnClicked += OnClicked;
		}

		protected virtual void OnDisable()
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
			UIManager.ShowDialog(this, () => _dialogUI.Setup(message, answers), hideOtherDialogs).Forget();
		}

		protected internal virtual async UniTask ShowAsync()
		{
			await _dialogUI.ShowAsync();
		}

		protected internal virtual async UniTask HideAsync()
		{
			await _dialogUI.HideAsync();
		}

		protected virtual void OnClicked()
		{
			UIManager.HideDialog(this).Forget();
		}
	}
}