using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

		protected UIManagerBase UIManager;
		protected IReadOnlyList<PanelBase> ActivePanels => _activePanels;
		private List<PanelBase> _activePanels;

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

		public async UniTask Show(CancellationToken cancellationToken = default)
		{
			await UIManager.ShowScreen(this, cancellationToken);
			await ShowDefaultPanels(cancellationToken);
		}

		public async UniTask Show<T>(bool additive, CancellationToken cancellationToken = default) where T : PanelBase
		{
			await UIManager.ShowScreen(this, cancellationToken);
			await ShowPanelAsync<T>(additive, cancellationToken: cancellationToken);
		}

		public async UniTask Hide<T>(CancellationToken cancellationToken) where T : PanelBase
		{
			if (IsPanelActive<T>())
			{
				await HidePanelAsync<T>(cancellationToken);
			}
		}

		public async UniTask Hide(CancellationToken cancellationToken = default)
		{
			await UIManager.HideScreen(this, cancellationToken);
		}

		protected virtual async UniTask HidePanelsAsync(CancellationToken cancellationToken = default)
		{
			if (_activePanels.Count == 0)
			{
				return;
			}

			UniTask[] hideTasks = new UniTask[_activePanels.Count];

			for (int i = 0; i < _activePanels.Count; i++)
			{
				hideTasks[i] = _activePanels[i].HideAsync(cancellationToken);
			}

			await UniTask.WhenAll(hideTasks);
		}

		protected bool IsPanelActive<T>() where T : PanelBase
		{
			return _activePanels.Contains(GetPanel<T>());
		}

		protected virtual async UniTask ShowPanelAsync<T>(bool additive = false,
			CancellationToken cancellationToken = default) where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				await ShowPanelAsync(panel, additive, cancellationToken);
			}
		}

		protected virtual async UniTask ShowPanelAsync(PanelBase panelType, bool additive = false,
			CancellationToken cancellationToken = default)
		{
			if (!additive && _activePanels.Count > 0)
			{
				UniTask[] hidePanels = new UniTask[_activePanels.Count];
				for (int i = 0; i < _activePanels.Count; i++)
				{
					hidePanels[i] = _activePanels[i].HideAsync(cancellationToken);
				}

				await UniTask.WhenAll(hidePanels);
			}

			await DoShowPanelAsync(panelType, cancellationToken);
		}

		protected virtual async UniTask HidePanelAsync<T>(CancellationToken cancellationToken = default)
			where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				await HidePanelAsync(panel, cancellationToken);
			}
		}

		protected virtual async UniTask HidePanelAsync(PanelBase panel, CancellationToken cancellationToken = default)
		{
			await DoHidePanelAsync(panel, cancellationToken);
		}

		protected T GetPanel<T>() where T : PanelBase
		{
			_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel);
			return panel as T;
		}

		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			await _screenUI.Show(cancellationToken);
		}

		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await HidePanelsAsync(cancellationToken);
			await _screenUI.Hide(cancellationToken);
		}

		private UniTask ShowDefaultPanels(CancellationToken cancellationToken = default)
		{
			if (_panels?.Length > 0)
			{
				if (_defaultPanels?.Length > 0)
				{
					UniTask[] panelsToShow = new UniTask[_defaultPanels.Length];
					for (int i = 0; i < _defaultPanels.Length; i++)
					{
						panelsToShow[i] = DoShowPanelAsync(_defaultPanels[i], cancellationToken);
					}

					return UniTask.WhenAll(panelsToShow);
				}
			}

			return UniTask.CompletedTask;
		}

		private async UniTask DoShowPanelAsync(PanelBase panel, CancellationToken cancellationToken = default)
		{
			await panel.ShowAsync(cancellationToken);
			if (!_activePanels.Contains(panel))
			{
				_activePanels.Add(panel);
			}
		}

		private async UniTask DoHidePanelAsync(PanelBase panel, CancellationToken cancellationToken = default)
		{
			await panel.HideAsync(cancellationToken);
			if (_activePanels.Contains(panel))
			{
				_activePanels.Remove(panel);
			}
		}
	}
}