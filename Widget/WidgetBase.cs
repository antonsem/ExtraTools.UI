using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace ExtraTools.UI.Widget
{
	public abstract class WidgetBase : MonoBehaviour
	{
		[SerializeField] protected WidgetUIBase _widgetUI;

		[PublicAPI]
		protected UIManagerBase UIManager;

		private WidgetTask _task;

		[PublicAPI]
		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			_widgetUI.Initialize(this);
		}

		[PublicAPI]
		public virtual UniTask Show(string text, float showTime = 1, Action callback = null,
			CancellationToken cancellationToken = default)
		{
			return UIManager.ShowWidget(new WidgetTask(this, text, showTime, callback, cancellationToken));
		}

		[PublicAPI]
		public virtual UniTask Hide(CancellationToken cancellationToken = default)
		{
			return UIManager.HideWidget(this, cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(WidgetTask task,
			CancellationToken cancellationToken = default)
		{
			_task = task;
			await _widgetUI.ShowAsync(task, cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			// A widget can outlive its scene while it waits to expire.
			if (!_widgetUI)
			{
				return;
			}

			await _widgetUI.HideAsync(cancellationToken);
		}

		protected internal virtual void OnClick()
		{
			_task.Callback?.Invoke();
		}
	}
}
