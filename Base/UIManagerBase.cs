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
		private Dictionary<Type, ScreenBase> _screens;
		private Dictionary<Type, DialogBase> _dialogs;
		private Dictionary<Type, WidgetBase> _widgets;

		private readonly List<DialogBase> _activeDialogs = new();
		private readonly Queue<WidgetTask> _widgetQueue = new();

		public ScreenBase ActiveScreen { get; private set; }
		public WidgetTask ActiveWidget { get; private set; }

		public IReadOnlyList<DialogBase> ActiveDialogs => _activeDialogs;

		private bool _isInitialized;

		#region Constants

		private const string NoScreenError =
			"<b>{0}</b> <color=red>does not have</color> a screen of type <b>{1}</b>!";

		private const string NoDialogError =
			"<b>{0}</b> <color=red>does not have</color> a dialog of type <b>{1}</b>!";

		private const string NoWidgetError =
			"<b>{0}</b> <color=red>does not have</color> a widget of type <b>{1}</b>!";

		#endregion

		protected virtual void Awake()
		{
			Initialize();
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

			_screens = new Dictionary<Type, ScreenBase>(screens.Length);
			foreach (ScreenBase screen in screens)
			{
				screen.Initialize(this);
				_screens.Add(screen.GetType(), screen);
			}

			DialogBase[] dialogs = transform.GetComponentsInChildren<DialogBase>();

			_dialogs = new Dictionary<Type, DialogBase>(dialogs.Length);
			foreach (DialogBase dialog in dialogs)
			{
				dialog.Initialize(this);
				_dialogs.Add(dialog.GetType(), dialog);
			}

			WidgetBase[] widgets = transform.GetComponentsInChildren<WidgetBase>();

			_widgets = new Dictionary<Type, WidgetBase>(widgets.Length);
			foreach (WidgetBase widget in widgets)
			{
				widget.Initialize(this);
				_widgets.Add(widget.GetType(), widget);
			}

			_isInitialized = true;
		}

		internal async UniTask ShowScreen(ScreenBase screen, CancellationToken cancellationToken = default)
		{
			if (ActiveScreen)
			{
				await ActiveScreen.HideAsync();
			}

			await screen.ShowAsync();
			ActiveScreen = screen;
		}

		internal async UniTask HideScreen(ScreenBase screen, CancellationToken cancellationToken = default)
		{
			await screen.HideAsync();

			if (screen == ActiveScreen)
			{
				ActiveScreen = null;
			}
		}

		internal async UniTask ShowDialog(DialogBase dialog, bool hideRest = true)
		{
			if (hideRest)
			{
				await HideAllDialogs();
			}

			dialog.transform.SetAsLastSibling();
			await dialog.ShowAsync();
			_activeDialogs.Add(dialog);
		}

		internal async UniTask HideDialog(DialogBase dialog)
		{
			await dialog.HideAsync();

			if (!_activeDialogs.Contains(dialog))
			{
				Debug.LogWarning($"Dialog {dialog} is not registered as active in {name}", this);
			}
			else
			{
				_activeDialogs.Remove(dialog);
			}
		}

		internal async UniTask HideAllDialogs()
		{
			UniTask[] tasks = new UniTask[_activeDialogs.Count];

			for (int i = 0; i < _activeDialogs.Count; i++)
			{
				tasks[i] = _activeDialogs[i].HideAsync();
			}

			await UniTask.WhenAll(tasks);
			_activeDialogs.Clear();
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
			await ActiveWidget.Base.ShowAsync(ActiveWidget);
			ActiveWidget = null;

			if (_widgetQueue.Count > 0)
			{
				PlayWidgetQueue().Forget();
			}
		}
	}
}