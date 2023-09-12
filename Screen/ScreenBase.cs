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
		[SerializeField] protected ScreenUIBase screenUI;
		[SerializeField] protected PanelBase[] panels;

		private PanelBase _activePanel;
		protected UIManagerBase UIManager;

		private Dictionary<Type, PanelBase> _panels;

		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			screenUI.Initialize(this);
			_panels = new Dictionary<Type, PanelBase>();
			foreach (var panel in panels)
			{
				_panels.Add(panel.GetType(), panel);
				panel.Initialize(this);
			}

			if (panels?.Length > 0)
			{
				ShowPanelAsync(panels[0]);
			}
		}

		public async void Show()
		{
			await UIManager.ShowScreen(this);

			if (panels?.Length > 0)
			{
				ShowPanelAsync(panels[0]);
			}
		}

		public async Task Hide()
		{
			await UIManager.HideScreen(this);
		}

		protected virtual async Task HidePanelsAsync()
		{
			var hideTasks = new List<Task>(panels.Length);

			foreach (var panel in panels)
			{
				hideTasks.Add(panel.HideAsync());
			}

			await Task.WhenAll(hideTasks);
		}

		protected virtual async void ShowPanelAsync<T>() where T : PanelBase
		{
			if (_activePanel)
			{
				await _activePanel.HideAsync();
			}

			if (_panels.TryGetValue(typeof(T), out var panel))
			{
				ShowPanelAsync(panel);
			}
		}

		protected T GetPanel<T>() where T : PanelBase
		{
			_panels.TryGetValue(typeof(T), out var panel);
			return panel as T;
		}

		internal async Task ShowAsync()
		{
			await screenUI.Show();
		}

		internal async Task HideAsync()
		{
			await HidePanelsAsync();
			await screenUI.Hide();
		}

		private async void ShowPanelAsync(PanelBase panel)
		{
			await panel.ShowAsync();
			_activePanel = panel;
		}
	}
}