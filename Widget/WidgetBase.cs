using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using UnityEngine;

namespace ExtraTools.UI.Widget
{
	public abstract class WidgetBase : MonoBehaviour
	{
		[SerializeField] protected WidgetUIBase _widgetUI;

		protected UIManagerBase UIManager;
		private WidgetTask _task;

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
			await _widgetUI.ShowAsync(task);

			await UniTask.Delay((int)(1000 * task.ShowTime), cancellationToken: cancellationToken)
				.SuppressCancellationThrow();

			await _widgetUI.HideAsync();
		}

		protected internal virtual void OnClick()
		{
			_task.Callback?.Invoke();
		}
	}
}