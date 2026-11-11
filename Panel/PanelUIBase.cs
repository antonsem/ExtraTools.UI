using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExtraTools.UI.Panel
{
	[RequireComponent(typeof(Canvas))]
	public abstract class PanelUIBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_canvas")] protected Canvas Canvas;

		[PublicAPI] protected PanelBase PanelBase;

		private CanvasGroup _canvasGroup;

		internal void SetInteractable(bool interactable)
		{
			Interactability.Set(this, ref _canvasGroup, interactable);
		}

		[PublicAPI]
		protected internal virtual void Initialize(PanelBase panelBase)
		{
			PanelBase = panelBase;
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			Canvas.enabled = true;
			await UniTask.CompletedTask;
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			Canvas.enabled = false;
			await UniTask.CompletedTask;
		}
	}
}
