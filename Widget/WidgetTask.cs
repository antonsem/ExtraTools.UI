using System;
using System.Threading;

namespace ExtraTools.UI.Widget
{
	public class WidgetTask
	{
		public readonly float ShowTime;
		public readonly string Text;
		public readonly Action Callback;
		public readonly WidgetBase Base;

		public CancellationToken CancellationToken => _cancellationTokenSource.Token;

		private CancellationTokenSource _cancellationTokenSource;

		internal WidgetTask(WidgetBase widgetBase, string text, float showTime, Action callback)
		{
			Base = widgetBase;
			Text = text;
			ShowTime = showTime;
			Callback = callback;
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public void StopTask()
		{
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
			_cancellationTokenSource = null;
		}
	}
}