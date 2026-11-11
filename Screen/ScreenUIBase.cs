using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ExtraTools.UI.Screen
{
	[RequireComponent(typeof(Canvas))]
	public abstract class ScreenUIBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_canvas")] protected Canvas Canvas;

		[SerializeField, PublicAPI] protected internal Selectable FirstSelected;

		[PublicAPI] protected ScreenBase ScreenBase;

		private CanvasGroup _canvasGroup;

		internal void SetInteractable(bool interactable)
		{
			Interactability.Set(this, ref _canvasGroup, interactable);
		}

		[PublicAPI]
		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			ScreenBase = screenBase;
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
