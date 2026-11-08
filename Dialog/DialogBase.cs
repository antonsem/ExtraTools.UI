using System.Threading;
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
		public virtual UniTask Show(string message, DialogAnswer[] answers = null, bool hideOtherDialogs = false,
			CancellationToken cancellationToken = default)
		{
			return UIManager.ShowDialog(this, () => _dialogUI.Setup(message, answers), hideOtherDialogs,
				cancellationToken);
		}

		[PublicAPI]
		public virtual UniTask Hide(CancellationToken cancellationToken = default)
		{
			return UIManager.HideDialog(this, cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			await _dialogUI.ShowAsync(cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await _dialogUI.HideAsync(cancellationToken);
		}

		[PublicAPI]
		protected virtual void OnClicked()
		{
			Hide().Forget();
		}
	}
}
