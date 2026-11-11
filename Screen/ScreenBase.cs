using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using ExtraTools.UI.Panel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExtraTools.UI.Screen
{
	public abstract class ScreenBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_screenUI")] protected ScreenUIBase ScreenUI;
		[SerializeField, FormerlySerializedAs("_panels")] protected PanelBase[] Panels;
		[SerializeField, FormerlySerializedAs("_defaultPanels")] protected PanelBase[] DefaultPanels;

		[PublicAPI] protected UIManagerBase UIManager;

		[PublicAPI] protected IReadOnlyList<PanelBase> ActivePanels => _activePanels;

		private List<PanelBase> _activePanels;

		private Dictionary<Type, PanelBase> _panelsDictionary;
		private readonly SemaphoreSlim _panelTransition = new(1, 1);
		private bool _isInteractable;

		private const string NoPanelError =
			"<b>{0}</b> <color=red>does not have</color> a panel of type <b>{1}</b>!";

		[PublicAPI]
		protected internal virtual void Initialize(UIManagerBase uiManager)
		{
			UIManager = uiManager;
			_activePanels = new List<PanelBase>(Panels.Length);
			ScreenUI.Initialize(this);
			_panelsDictionary = new Dictionary<Type, PanelBase>();
			foreach (PanelBase panel in Panels)
			{
				_panelsDictionary.Add(panel.GetType(), panel);
				panel.Initialize(this);
			}

			ShowDefaultPanels().Forget();
		}

		internal void SetInteractable(bool interactable)
		{
			_isInteractable = interactable;
			ScreenUI.SetInteractable(interactable);

			foreach (PanelBase panel in Panels)
			{
				panel.SetInteractable(interactable && _activePanels.Contains(panel));
			}
		}

		[PublicAPI]
		public async UniTask Show(CancellationToken cancellationToken = default)
		{
			if (UIManager.ActiveScreen == this)
			{
				await HidePanelsAsync(cancellationToken);
			}

			await UIManager.ShowScreen(this, cancellationToken);
			await ShowDefaultPanels(cancellationToken);
		}

		[PublicAPI]
		public async UniTask Show<T>(bool additive = false, CancellationToken cancellationToken = default)
			where T : PanelBase
		{
			await UIManager.ShowScreen(this, cancellationToken);
			await ShowPanelAsync<T>(additive, cancellationToken);
		}

		[PublicAPI]
		public async UniTask Hide<T>(CancellationToken cancellationToken = default) where T : PanelBase
		{
			if (IsPanelActive<T>())
			{
				await HidePanelAsync<T>(cancellationToken);
			}
		}

		[PublicAPI]
		public async UniTask Hide(CancellationToken cancellationToken = default)
		{
			await UIManager.HideScreen(this, cancellationToken);
		}

		[PublicAPI]
		protected virtual async UniTask HidePanelsAsync(CancellationToken cancellationToken = default)
		{
			await _panelTransition.WaitAsync(cancellationToken);

			try
			{
				if (_activePanels.Count == 0)
				{
					return;
				}

				await DoHideActivePanelsAsync(cancellationToken);
			}
			finally
			{
				_panelTransition.Release();
			}
		}

		[PublicAPI]
		protected bool IsPanelActive<T>() where T : PanelBase
		{
			return _activePanels.Contains(GetPanel<T>());
		}

		[PublicAPI]
		protected virtual async UniTask ShowPanelAsync<T>(bool additive = false,
			CancellationToken cancellationToken = default) where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				await ShowPanelAsync(panel, additive, cancellationToken);
			}
			else
			{
				Debug.LogError(string.Format(NoPanelError, name, typeof(T)), this);
			}
		}

		[PublicAPI]
		protected virtual async UniTask ShowPanelAsync(PanelBase panelType, bool additive = false,
			CancellationToken cancellationToken = default)
		{
			// One at a time: overlapping animated changes would miss panels that are still being shown.
			await _panelTransition.WaitAsync(cancellationToken);

			try
			{
				if (!additive && _activePanels.Count > 0)
				{
					await DoHideActivePanelsAsync(cancellationToken);
				}

				await DoShowPanelAsync(panelType, cancellationToken);
			}
			finally
			{
				_panelTransition.Release();
			}
		}

		[PublicAPI]
		protected virtual async UniTask HidePanelAsync<T>(CancellationToken cancellationToken = default)
			where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				await HidePanelAsync(panel, cancellationToken);
			}
			else
			{
				Debug.LogError(string.Format(NoPanelError, name, typeof(T)), this);
			}
		}

		[PublicAPI]
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

		[PublicAPI]
		protected T GetPanel<T>() where T : PanelBase
		{
			if (_panelsDictionary.TryGetValue(typeof(T), out PanelBase panel))
			{
				return panel as T;
			}

			Debug.LogError(string.Format(NoPanelError, name, typeof(T)), this);
			return null;
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			await ScreenUI.ShowAsync(cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await HidePanelsAsync(cancellationToken);
			await ScreenUI.HideAsync(cancellationToken);
		}

		private async UniTask ShowDefaultPanels(CancellationToken cancellationToken = default)
		{
			if (Panels?.Length > 0 && DefaultPanels?.Length > 0)
			{
				await _panelTransition.WaitAsync(cancellationToken);

				try
				{
					UniTask[] panelsToShow = new UniTask[DefaultPanels.Length];
					for (int i = 0; i < DefaultPanels.Length; i++)
					{
						panelsToShow[i] = DoShowPanelAsync(DefaultPanels[i], cancellationToken);
					}

					await UniTask.WhenAll(panelsToShow);
				}
				finally
				{
					_panelTransition.Release();
				}
			}
		}

		// Must be called while holding _panelTransition, which is not reentrant.
		private async UniTask DoHideActivePanelsAsync(CancellationToken cancellationToken)
		{
			// A copy, because each panel removes itself from the list once its hide finishes.
			PanelBase[] panels = _activePanels.ToArray();
			UniTask[] hideTasks = new UniTask[panels.Length];

			for (int i = 0; i < panels.Length; i++)
			{
				hideTasks[i] = DoHidePanelAsync(panels[i], cancellationToken);
			}

			await UniTask.WhenAll(hideTasks);
		}

		private async UniTask DoShowPanelAsync(PanelBase panel, CancellationToken cancellationToken = default)
		{
			if (!_activePanels.Contains(panel))
			{
				_activePanels.Add(panel);
			}

			panel.SetInteractable(_isInteractable);
			await panel.ShowAsync(cancellationToken);
		}

		private async UniTask DoHidePanelAsync(PanelBase panel, CancellationToken cancellationToken = default)
		{
			panel.SetInteractable(false);
			await panel.HideAsync(cancellationToken);
			if (_activePanels.Contains(panel))
			{
				_activePanels.Remove(panel);
			}
		}
	}
}
