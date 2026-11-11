using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ExtraTools.UI.Dialog
{
	public abstract class DialogBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_dialogUI")] protected DialogUIBase DialogUI;

		[PublicAPI] protected UIManagerBase UIManager;

		#region Unity Methods

		[PublicAPI]
		protected virtual void OnEnable()
		{
			DialogUI.OnClicked += OnClicked;
		}

		[PublicAPI]
		protected virtual void OnDisable()
		{
			DialogUI.OnClicked -= OnClicked;
		}

		#endregion

		internal Selectable FirstAnswer => DialogUI.FirstAnswer;

		internal void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
		}

		internal bool Contains(GameObject target)
		{
			return DialogUI.Contains(target);
		}

		internal void SetInteractable(bool interactable)
		{
			DialogUI.SetInteractable(interactable);
		}

		[PublicAPI]
		public virtual UniTask Show(string message, DialogAnswer[] answers = null, bool hideOtherDialogs = false,
			CancellationToken cancellationToken = default)
		{
			return UIManager.ShowDialog(this, () => DialogUI.Setup(message, answers), hideOtherDialogs,
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
			await DialogUI.ShowAsync(cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await DialogUI.HideAsync(cancellationToken);
		}

		[PublicAPI]
		protected virtual void OnClicked()
		{
			Hide().Forget();
		}
	}
}
