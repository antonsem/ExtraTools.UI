using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

		private ScreenBase _activeScreen;
		private readonly List<DialogBase> _activeDialogs = new();
		private readonly Queue<WidgetTask> _widgetQueue = new();
		private WidgetTask _activeWidget;

		private bool _isInitialized;

		#region Constants

		private const string NO_SCREEN_ERROR =
			"<b>{0}</b> <color=red>does not have</color> a screen of type <b>{1}</b>!";

		private const string NO_DIALOG_ERROR =
			"<b>{0}</b> <color=red>does not have</color> a dialog of type <b>{1}</b>!";

		private const string NO_WIDGET_ERROR =
			"<b>{0}</b> <color=red>does not have</color> a widget of type <b>{1}</b>!";

		#endregion


		private void Awake()
		{
			Initialize();
		}

		public T GetScreen<T>() where T : ScreenBase
		{
			if (!_isInitialized)
			{
				Initialize();
			}

			_screens.TryGetValue(typeof(T), out var val);

			if (!val)
			{
				Debug.LogError(string.Format(NO_SCREEN_ERROR, name, typeof(T)), this);
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

			_dialogs.TryGetValue(typeof(T), out var val);

			if (!val)
			{
				Debug.LogError(string.Format(NO_DIALOG_ERROR, name, typeof(T)), this);
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

			_widgets.TryGetValue(typeof(T), out var val);

			if (!val)
			{
				Debug.LogError(string.Format(NO_WIDGET_ERROR, name, typeof(T)), this);
				return null;
			}

			return val.GetComponent<T>();
		}

		private void Initialize()
		{
			var screens = transform.GetComponentsInChildren<ScreenBase>();

			_screens = new Dictionary<Type, ScreenBase>(screens.Length);
			foreach (var screen in screens)
			{
				screen.Initialize(this);
				_screens.Add(screen.GetType(), screen);
			}

			var dialogs = transform.GetComponentsInChildren<DialogBase>();

			_dialogs = new Dictionary<Type, DialogBase>(dialogs.Length);
			foreach (var dialog in dialogs)
			{
				dialog.Initialize(this);
				_dialogs.Add(dialog.GetType(), dialog);
			}

			var widgets = transform.GetComponentsInChildren<WidgetBase>();

			_widgets = new Dictionary<Type, WidgetBase>(widgets.Length);
			foreach (var widget in widgets)
			{
				widget.Initialize(this);
				_widgets.Add(widget.GetType(), widget);
			}

			_isInitialized = true;
		}

		internal async Task ShowScreen(ScreenBase screen)
		{
			if (_activeScreen != null)
			{
				await _activeScreen.HideAsync();
			}

			await screen.ShowAsync();
			_activeScreen = screen;
		}

		internal async Task HideScreen(ScreenBase screen)
		{
			await screen.HideAsync();

			if (screen == _activeScreen)
			{
				_activeScreen = null;
			}
		}

		internal async void ShowDialog(DialogBase dialog, bool hideRest = true)
		{
			if (hideRest)
			{
				await HideAllDialogs();
			}

			dialog.transform.SetAsLastSibling();
			await dialog.ShowAsync();
			_activeDialogs.Add(dialog);
		}

		internal async void HideDialog(DialogBase dialog)
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

		internal async Task HideAllDialogs()
		{
			var tasks = new Task[_activeDialogs.Count];

			for (var i = 0; i < _activeDialogs.Count; i++)
			{
				tasks[i] = _activeDialogs[i].HideAsync();
			}

			await Task.WhenAll(tasks);
			_activeDialogs.Clear();
		}

		internal void ShowWidget(WidgetTask task)
		{
			_widgetQueue.Enqueue(task);
			PlayWidgetQueue();
		}

		internal void HideAllWidgets()
		{
			_widgetQueue.Clear();
		}

		private async void PlayWidgetQueue()
		{
			if (_activeWidget != null)
			{
				return;
			}

			_activeWidget = _widgetQueue.Dequeue();
			await _activeWidget.Base.ShowAsync(_activeWidget);
			_activeWidget = null;

			if (_widgetQueue.Count > 0)
			{
				PlayWidgetQueue();
			}
		}
	}
}