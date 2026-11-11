using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ExtraTools.UI.Widget
{
	[RequireComponent(typeof(Canvas))]
	public abstract class WidgetUIBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_canvas")] protected Canvas Canvas;
		[SerializeField, FormerlySerializedAs("_text")] protected TMP_Text Text;
		[SerializeField] private Button _button;

		[PublicAPI] protected WidgetBase Widget;

		private CanvasGroup _canvasGroup;

		internal void SetInteractable(bool interactable)
		{
			Interactability.Set(this, ref _canvasGroup, interactable);
		}

		[PublicAPI]
		protected virtual void OnEnable()
		{
			_button.onClick.AddListener(OnClick);
		}

		[PublicAPI]
		protected virtual void OnDisable()
		{
			_button.onClick.RemoveAllListeners();
		}

		[PublicAPI]
		protected virtual void OnClick()
		{
			Widget.OnClick();
		}

		[PublicAPI]
		protected internal virtual void Initialize(WidgetBase widget)
		{
			Widget = widget;
		}

		[PublicAPI]
		protected internal virtual UniTask ShowAsync(WidgetTask task, CancellationToken cancellationToken = default)
		{
			Text.text = task.Text;
			Canvas.enabled = true;

			return UniTask.CompletedTask;
		}

		[PublicAPI]
		protected internal virtual UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			Canvas.enabled = false;
			return UniTask.CompletedTask;
		}
	}
}
