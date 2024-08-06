using System;
using System.Threading.Tasks;
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

		protected internal virtual async Task ShowAsync(WidgetTask task)
		{
			_task = task;
			await _widgetUI.ShowAsync(task);

			try
			{
				await Task.Delay((int)(1000 * task.ShowTime), task.CancellationToken);
			}
			catch (TaskCanceledException)
			{
			}

			await _widgetUI.HideAsync();
		}

		protected internal virtual void OnClick()
		{
			_task.Callback?.Invoke();
			_task.StopTask();
		}
	}
}