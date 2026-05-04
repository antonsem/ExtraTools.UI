using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Screen;
using UnityEngine;

namespace ExtraTools.UI.Panel
{
	public abstract class PanelBase : MonoBehaviour
	{
		[SerializeField] protected PanelUIBase _panelUI;

		protected ScreenBase Screen;

		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			Screen = screenBase;
			_panelUI.Initialize(this);
		}

		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			await _panelUI.ShowAsync(cancellationToken);
		}

		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await _panelUI.HideAsync(cancellationToken);
		}
	}
}
