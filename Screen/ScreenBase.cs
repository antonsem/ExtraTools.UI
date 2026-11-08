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
		private readonly SemaphoreSlim _panelTransition = new(1, 1);

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

			ShowDefaultPanels().Forget();
		}

		public async UniTask Show(CancellationToken cancellationToken = default)
		{
			if (UIManager.ActiveScreen == this)
			{
				await HidePanelsAsync(cancellationToken);
			}

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
			await _panelTransition.WaitAsync(cancellationToken);

			try
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

				_activePanels.Clear();

				await UniTask.WhenAll(hideTasks);
			}
			finally
			{
				_panelTransition.Release();
			}
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
			// One at a time: overlapping animated changes would miss panels that are still being shown.
			await _panelTransition.WaitAsync(cancellationToken);

			try
			{
				if (!additive && _activePanels.Count > 0)
				{
					UniTask[] hidePanels = new UniTask[_activePanels.Count];
					for (int i = _activePanels.Count - 1; i >= 0; i--)
					{
						hidePanels[i] = _activePanels[i].HideAsync(cancellationToken);
						_activePanels.RemoveAt(i);
					}

					await UniTask.WhenAll(hidePanels);
				}

				await DoShowPanelAsync(panelType, cancellationToken);
			}
			finally
			{
				_panelTransition.Release();
			}
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
			await _panelTransition.WaitAsync(cancellationToken);

			try
			{
				await DoHidePanelAsync(panel, cancellationToken);
			}
			finally
			{
				_panelTransition.Release();
			}
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

		private async UniTask ShowDefaultPanels(CancellationToken cancellationToken = default)
		{
			if (_panels?.Length > 0 && _defaultPanels?.Length > 0)
			{
				await _panelTransition.WaitAsync(cancellationToken);

				try
				{
					UniTask[] panelsToShow = new UniTask[_defaultPanels.Length];
					for (int i = 0; i < _defaultPanels.Length; i++)
					{
						panelsToShow[i] = DoShowPanelAsync(_defaultPanels[i], cancellationToken);
					}

					await UniTask.WhenAll(panelsToShow);
				}
				finally
				{
					_panelTransition.Release();
				}
			}
		}

		private async UniTask DoShowPanelAsync(PanelBase panel, CancellationToken cancellationToken = default)
		{
			// Added before the await so that a cancelled show can still be hidden later.
			if (!_activePanels.Contains(panel))
			{
				_activePanels.Add(panel);
			}

			await panel.ShowAsync(cancellationToken);
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