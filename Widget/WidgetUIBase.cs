using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraTools.UI.Widget
{
	[RequireComponent(typeof(Canvas))]
	public abstract class WidgetUIBase : MonoBehaviour
	{
		[SerializeField] protected Canvas _canvas;
		[SerializeField] protected TMP_Text _text;
		[SerializeField] private Button _button;

		[PublicAPI]
		protected WidgetBase Widget;

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
			_text.text = task.Text;
			_canvas.enabled = true;

			return UniTask.CompletedTask;
		}

		[PublicAPI]
		protected internal virtual UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = false;
			return UniTask.CompletedTask;
		}
	}
}
