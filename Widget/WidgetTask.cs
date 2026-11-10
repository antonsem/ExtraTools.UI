using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace ExtraTools.UI.Widget
{
	public class WidgetTask
	{
		public readonly float ShowTime;
		public readonly string Text;
		public readonly Action Callback;
		public readonly WidgetBase Base;

		public CancellationToken CancellationToken => _cancellationTokenSource.Token;

		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly UniTaskCompletionSource _shown = new();
		private readonly UniTaskCompletionSource _hidden = new();
		private CancellationTokenRegistration _externalCancellation;
		private bool _isStarted;

		internal UniTask Shown => _shown.Task;
		internal UniTask Hidden => _hidden.Task;
		internal bool IsStopped => _cancellationTokenSource.IsCancellationRequested;

		internal WidgetTask(WidgetBase widgetBase, string text, float showTime, Action callback,
			CancellationToken cancellationToken = default)
		{
			Base = widgetBase;
			Text = text;
			ShowTime = showTime;
			Callback = callback;
			_cancellationTokenSource = new CancellationTokenSource();

			// Last, because an already canceled token runs StopTask right here.
			_externalCancellation = cancellationToken.Register(StopTask);
		}

		[PublicAPI]
		public void StopTask()
		{
			if (_cancellationTokenSource.IsCancellationRequested)
			{
				return;
			}

			_cancellationTokenSource.Cancel();

			// A queued widget never plays, so nothing else would release whoever is awaiting it.
			if (!_isStarted)
			{
				MarkFinished();
			}
		}

		internal void MarkStarted()
		{
			_isStarted = true;
		}

		internal void MarkShown()
		{
			_shown.TrySetResult();
		}

		internal void MarkFinished()
		{
			_externalCancellation.Dispose();
			_shown.TrySetResult();
			_hidden.TrySetResult();
		}
	}
}
