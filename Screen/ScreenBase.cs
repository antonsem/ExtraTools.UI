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

		private PanelBase _activePanel;
		protected UIManagerBase UIManager;

		private Dictionary<Type, PanelBase> _panelsDictionary;

		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			_screenUI.Initialize(this);
			_panelsDictionary = new Dictionary<Type, PanelBase>();
			foreach (PanelBase panel in _panels)
			{
				_panelsDictionary.Add(panel.GetType(), panel);
				panel.Initialize(this);
			}

			if(_panels?.Length > 0)
			{
				ShowPanelAsync(_panels[0]);
			}
		}

		public async void Show()
		{
			await UIManager.ShowScreen(this);

			if(_panels?.Length > 0)
			{
				ShowPanelAsync(_panels[0]);
			}
		}

		public async Task Hide()
		{
			await UIManager.HideScreen(this);
		}

		protected virtual async Task HidePanelsAsync()
		{
			List<Task> hideTasks = new(_panels.Length);

			foreach (PanelBase panel in _panels)
			{
				hideTasks.Add(panel.HideAsync());
			}

			await Task.WhenAll(hideTasks);
		}

		protected virtual async void ShowPanelAsync<T>() where T : PanelBase
		{
			if(_activePanel)
			{
				await _activePanel.HideAsync();
			}

			if(_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				ShowPanelAsync(panel);
			}
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

		private async void ShowPanelAsync(PanelBase panel)
		{
			await panel.ShowAsync();
			_activePanel = panel;
		}

#if UNITY_EDITOR
		[ContextMenu("Set Screen")]
		internal void SetScreen()
		{
			_screenUI = GetComponent<ScreenUIBase>();
			_screenUI.SetScreenUI();

			_panels = transform.GetComponentsInChildren<PanelBase>();
			foreach (PanelBase panel in _panels)
			{
				panel.SetPanel();
			}
		}

		private void Reset()
		{
			SetScreen();
		}
#endif
	}
}
