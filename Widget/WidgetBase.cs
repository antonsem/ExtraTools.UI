using System;
using System.Threading.Tasks;
using ExtraTools.UI.Base;
using UnityEngine;

namespace ExtraTools.UI.Widget
{
	public abstract class WidgetBase : MonoBehaviour
	{
		[SerializeField] private WidgetUIBase widgetUI;

		protected UIManagerBase UIManager;
		private Action _callback;

		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			widgetUI.Initialize(this);
		}

		public virtual void Show(string text, float showTime = 1, Action callback = null)
		{
			UIManager.ShowWidget(new WidgetTask(this, text, showTime, callback));
		}

		protected internal virtual async Task ShowAsync(WidgetTask task)
		{
			_callback = task.Callback;
			await widgetUI.ShowAsync(task);
		}

		protected internal virtual void OnClick()
		{
			_callback?.Invoke();
		}
	}
}