using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExtraTools.UI.Base;
using ExtraTools.UI.Panel;
using UnityEngine;

namespace ExtraTools.UI.Screen
{
	public abstract class ScreenBase : MonoBehaviour
	{
		[SerializeField] protected ScreenUIBase _screenUI;
		[SerializeField] protected PanelBase[] _panels;
		[SerializeField] protected PanelBase[] _defaultPanels;

		private List<PanelBase> _activePanels;
		protected UIManagerBase UIManager;

		private Dictionary<Type, PanelBase> _panelsDictionary;

		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			_activePanels = new List<PanelBase>(_panels.Length);
			_screenUI.Initialize(this);
			_panelsDictionary = new Dictionary<Type, PanelBase>();
			foreach (PanelBase panel in _panels)
			{
				_panelsDictionary.Add(panel.GetType(), panel);
				panel.Initialize(this);
			}

			ShowDefaultPanels();
		}

		public async void Show()
		{
			await UIManager.ShowScreen(this);
			await ShowDefaultPanels();
		}

		public async void Show<T>() where T : PanelBase
		{
			await UIManager.ShowScreen(this);
			await ShowPanelAsync<T>();
		}

		public async void Hide()
		{
			await UIManager.HideScreen(this);
		}

		protected virtual async Task HidePanelsAsync()
		{
			if (_activePanels.Count == 0)
			{
				return;
			}

			Task[] hideTasks = new Task[_activePanels.Count];

			for (int i = 0; i < _activePanels.Count; i++)
			{
				hideTasks[i] = _activePanels[i].HideAsync();
			}

			await Task.WhenAll(hideTasks);
		}

		protected virtual async Task ShowPanelAsync<T>(bool additive = false) where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				await ShowPanelAsync(panel, additive);
			}
		}

		protected virtual async Task ShowPanelAsync(PanelBase panelType, bool additive = false)
		{
			if (!additive && _activePanels.Count > 0)
			{
				Task[] hidePanels = new Task[_activePanels.Count];
				for (int i = 0; i < _activePanels.Count; i++)
				{
					hidePanels[i] = _activePanels[i].HideAsync();
				}

				await Task.WhenAll(hidePanels);
			}

			await DoShowPanelAsync(panelType);
		}

		protected virtual async Task HidePanelAsync<T>() where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				await HidePanelAsync(panel);
			}
		}

		protected virtual async Task HidePanelAsync(PanelBase panel)
		{
			await DoHidePanelAsync(panel);
		}

		protected T GetPanel<T>() where T : PanelBase
		{
			_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel);
			return panel as T;
		}

		internal async Task ShowAsync()
		{
			await _screenUI.Show();
		}

		internal async Task HideAsync()
		{
			await HidePanelsAsync();
			await _screenUI.Hide();
		}

		private Task ShowDefaultPanels()
		{
			if (_panels?.Length > 0)
			{
				if (_defaultPanels?.Length > 0)
				{
					Task[] panelsToShow = new Task[_defaultPanels.Length];
					for (int i = 0; i < _defaultPanels.Length; i++)
					{
						panelsToShow[i] = DoShowPanelAsync(_defaultPanels[i]);
					}

					return Task.WhenAll(panelsToShow);
				}
			}

			return Task.CompletedTask;
		}

		private async Task DoShowPanelAsync(PanelBase panel)
		{
			await panel.ShowAsync();
			if (!_activePanels.Contains(panel))
			{
				_activePanels.Add(panel);
			}
		}

		private async Task DoHidePanelAsync(PanelBase panel)
		{
			await panel.HideAsync();
			if (_activePanels.Contains(panel))
			{
				_activePanels.Remove(panel);
			}
		}
	}
}