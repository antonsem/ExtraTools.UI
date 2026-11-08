using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using UnityEngine;

namespace ExtraTools.UI.Widget
{
	public abstract class WidgetBase : MonoBehaviour
	{
		protected UIManagerBase UIManager;
		private WidgetTask _task;
		[SerializeField] protected WidgetUIBase _widgetUI;

		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			_widgetUI.Initialize(this);
		}

		public virtual void Show(string text, float showTime = 1, Action callback = null)
		{
			UIManager.ShowWidget(new WidgetTask(this, text, showTime, callback));
		}

		protected internal virtual async UniTask ShowAsync(WidgetTask task,
			CancellationToken cancellationToken = default)
		{
			_task = task;

			// A stop can cancel the show animation, but the widget still has to hide afterwards.
			bool isCancelled = await _widgetUI.ShowAsync(task, cancellationToken).SuppressCancellationThrow();

			if (!isCancelled)
			{
				await UniTask.Delay((int)(1000 * task.ShowTime), true, cancellationToken: cancellationToken)
					.SuppressCancellationThrow();
			}

			if (!_widgetUI)
			{
				return;
			}

			// ReSharper disable once MethodSupportsCancellation
			// The token is left out on purpose: after a stop it is already canceled and the hide must still finish.
			await _widgetUI.HideAsync();
		}

		protected internal virtual void OnClick()
		{
			_task.Callback?.Invoke();
		}
	}
}
