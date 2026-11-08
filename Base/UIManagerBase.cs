using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Dialog;
using ExtraTools.UI.Screen;
using ExtraTools.UI.Widget;
using UnityEngine;

namespace ExtraTools.UI.Base
{
	public abstract class UIManagerBase : MonoBehaviour
	{
		private const string NoScreenError =
			"<b>{0}</b> <color=red>does not have</color> a screen of type <b>{1}</b>!";

		private const string NoDialogError =
			"<b>{0}</b> <color=red>does not have</color> a dialog of type <b>{1}</b>!";

		private const string NoWidgetError =
			"<b>{0}</b> <color=red>does not have</color> a widget of type <b>{1}</b>!";
		private readonly List<DialogBase> _activeDialogs = new();
		private readonly SemaphoreSlim _dialogTransition = new(1, 1);
		private readonly SemaphoreSlim _screenTransition = new(1, 1);
		private readonly Queue<WidgetTask> _widgetQueue = new();
		private Dictionary<Type, DialogBase> _dialogs;

		private bool _isInitialized;
		private Dictionary<Type, ScreenBase> _screens;
		private Dictionary<Type, WidgetBase> _widgets;

		public ScreenBase ActiveScreen { get; private set; }

		public WidgetTask ActiveWidget { get; private set; }

		public IReadOnlyList<DialogBase> ActiveDialogs => _activeDialogs;

		protected virtual void Awake()
		{
			if (!_isInitialized)
			{
				Initialize();
			}
		}

		protected virtual void OnDestroy()
		{
			HideAllWidgets();
		}

		public bool IsDialogActive(DialogBase dialog)
		{
			return _activeDialogs.Contains(dialog);
		}

		public T GetScreen<T>() where T : ScreenBase
		{
			if (!_isInitialized)
			{
				Initialize();
			}

			_screens.TryGetValue(typeof(T), out ScreenBase val);

			if (!val)
			{
				Debug.LogError(string.Format(NoScreenError, name, typeof(T)), this);
				return null;
			}

			return val.GetComponent<T>();
		}

		public T GetDialog<T>() where T : DialogBase
		{
			if (!_isInitialized)
			{
				Initialize();
			}

			_dialogs.TryGetValue(typeof(T), out DialogBase val);

			if (!val)
			{
				Debug.LogError(string.Format(NoDialogError, name, typeof(T)), this);
				return null;
			}

			return val.GetComponent<T>();
		}

		public T GetWidget<T>() where T : WidgetBase
		{
			if (!_isInitialized)
			{
				Initialize();
			}

			_widgets.TryGetValue(typeof(T), out WidgetBase val);

			if (!val)
			{
				Debug.LogError(string.Format(NoWidgetError, name, typeof(T)), this);
				return null;
			}

			return val.GetComponent<T>();
		}

		private void Initialize()
		{
			ScreenBase[] screens = transform.GetComponentsInChildren<ScreenBase>();
			DialogBase[] dialogs = transform.GetComponentsInChildren<DialogBase>();
			WidgetBase[] widgets = transform.GetComponentsInChildren<WidgetBase>();

			_screens = new Dictionary<Type, ScreenBase>(screens.Length);
			foreach (ScreenBase screen in screens)
			{
				_screens.Add(screen.GetType(), screen);
			}

			_dialogs = new Dictionary<Type, DialogBase>(dialogs.Length);
			foreach (DialogBase dialog in dialogs)
			{
				_dialogs.Add(dialog.GetType(), dialog);
			}

			_widgets = new Dictionary<Type, WidgetBase>(widgets.Length);
			foreach (WidgetBase widget in widgets)
			{
				_widgets.Add(widget.GetType(), widget);
			}

			// Everything is registered first because a child's Initialize override may look up any other UI.
			_isInitialized = true;

			// Screens go last so they can already use dialogs and widgets.
			foreach (DialogBase dialog in dialogs)
			{
				dialog.Initialize(this);
			}

			foreach (WidgetBase widget in widgets)
			{
				widget.Initialize(this);
			}

			foreach (ScreenBase screen in screens)
			{
				screen.Initialize(this);
			}
		}

		internal async UniTask ShowScreen(ScreenBase screen, CancellationToken cancellationToken = default)
		{
			// One at a time: overlapping animated transitions would each act on the same ActiveScreen and leave several
			// screens visible.
			await _screenTransition.WaitAsync(cancellationToken);

			try
			{
				if (ActiveScreen == screen)
				{
					return;
				}

				if (ActiveScreen)
				{
					await ActiveScreen.HideAsync(cancellationToken);
				}

				// Set before the await so that a canceled show can still be hidden by the next screen.
				ActiveScreen = screen;
				await screen.ShowAsync(cancellationToken);
			}
			finally
			{
				_screenTransition.Release();
			}
		}

		internal async UniTask HideScreen(ScreenBase screen, CancellationToken cancellationToken = default)
		{
			await _screenTransition.WaitAsync(cancellationToken);

			try
			{
				await screen.HideAsync(cancellationToken);

				if (screen == ActiveScreen)
				{
					ActiveScreen = null;
				}
			}
			finally
			{
				_screenTransition.Release();
			}
		}

		internal async UniTask ShowDialog(DialogBase dialog, Action setup, bool hideRest)
		{
			// One at a time, and hiding the rest shares the turn with the show: otherwise two calls that each hide the
			// others would interleave and leave both dialogs open.
			await _dialogTransition.WaitAsync();

			try
			{
				if (hideRest)
				{
					await HideAllDialogs();
				}

				// After the hide, because hiding clears a dialog's buttons and this dialog may be one of those hidden.
				setup();
				dialog.transform.SetAsLastSibling();

				if (_activeDialogs.Contains(dialog))
				{
					return;
				}

				// Added before the await so that a failed show can still be hidden later.
				_activeDialogs.Add(dialog);
				await dialog.ShowAsync();
			}
			finally
			{
				_dialogTransition.Release();
			}
		}

		internal async UniTask HideDialog(DialogBase dialog)
		{
			await _dialogTransition.WaitAsync();

			try
			{
				if (!_activeDialogs.Remove(dialog))
				{
					Debug.LogWarning($"Dialog {dialog} is not registered as active in {name}", this);
					return;
				}

				await dialog.HideAsync();
			}
			finally
			{
				_dialogTransition.Release();
			}
		}

		// Must be called while holding _dialogTransition, which is not reentrant.
		private async UniTask HideAllDialogs()
		{
			UniTask[] tasks = new UniTask[_activeDialogs.Count];

			for (int i = 0; i < _activeDialogs.Count; i++)
			{
				tasks[i] = _activeDialogs[i].HideAsync();
			}

			// Cleared before the await: a dialog that stays listed after being hidden could never be shown again.
			_activeDialogs.Clear();
			await UniTask.WhenAll(tasks);
		}

		internal void ShowWidget(WidgetTask task)
		{
			_widgetQueue.Enqueue(task);
			PlayWidgetQueue().Forget();
		}

		protected void HideActiveWidget()
		{
			ActiveWidget?.StopTask();
		}

		protected void HideAllWidgets()
		{
			_widgetQueue.Clear();
			HideActiveWidget();
		}

		private async UniTask PlayWidgetQueue()
		{
			if (ActiveWidget != null)
			{
				return;
			}

			ActiveWidget = _widgetQueue.Dequeue();

			try
			{
				await ActiveWidget.Base.ShowAsync(ActiveWidget, ActiveWidget.CancellationToken);
			}
			catch (OperationCanceledException)
			{
				// Stopping a widget is expected, and subclasses overriding ShowAsync may let this through.
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, this);
			}

			ActiveWidget = null;

			if (_widgetQueue.Count > 0)
			{
				PlayWidgetQueue().Forget();
			}
		}
	}
}
