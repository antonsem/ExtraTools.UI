using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Screen;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExtraTools.UI.Panel
{
	public abstract class PanelBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_panelUI")] protected PanelUIBase PanelUI;

		[PublicAPI] protected ScreenBase Screen;

		[PublicAPI]
		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			Screen = screenBase;
			PanelUI.Initialize(this);
		}

		internal void SetInteractable(bool interactable)
		{
			PanelUI.SetInteractable(interactable);
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			await PanelUI.ShowAsync(cancellationToken);
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			await PanelUI.HideAsync(cancellationToken);
		}
	}
}
