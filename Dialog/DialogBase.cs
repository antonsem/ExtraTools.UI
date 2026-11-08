using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace ExtraTools.UI.Dialog
{
	public abstract class DialogBase : MonoBehaviour
	{
		[SerializeField] protected DialogUIBase _dialogUI;

		protected UIManagerBase UIManager;

		#region Unity Methods

		[PublicAPI]
		protected virtual void OnEnable()
		{
			_dialogUI.OnClicked += OnClicked;
		}

		[PublicAPI]
		protected virtual void OnDisable()
		{
			_dialogUI.OnClicked -= OnClicked;
		}

		#endregion

		internal void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
		}

		[PublicAPI]
		public virtual void Show(string message, DialogAnswer[] answers = null, bool hideOtherDialogs = false)
		{
			UIManager.ShowDialog(this, () => _dialogUI.Setup(message, answers), hideOtherDialogs).Forget();
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync()
		{
			await _dialogUI.ShowAsync();
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync()
		{
			await _dialogUI.HideAsync();
		}

		[PublicAPI]
		protected virtual void OnClicked()
		{
			UIManager.HideDialog(this).Forget();
		}
	}
}
