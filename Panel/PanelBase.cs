using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Screen;
using JetBrains.Annotations;
using UnityEngine;

namespace ExtraTools.UI.Panel
{
	public abstract class PanelBase : MonoBehaviour
	{
		[SerializeField] protected PanelUIBase _panelUI;

		[PublicAPI]
		protected ScreenBase Screen;

		[PublicAPI]
		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			Screen = screenBase;
			_panelUI.Initialize(this);
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			await _panelUI.ShowAsync(cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await _panelUI.HideAsync(cancellationToken);
		}
	}
}
