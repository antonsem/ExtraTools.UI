using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace ExtraTools.UI.Panel
{
	[RequireComponent(typeof(Canvas))]
	public abstract class PanelUIBase : MonoBehaviour
	{
		[SerializeField] protected Canvas _canvas;

		[PublicAPI]
		protected PanelBase PanelBase;

		[PublicAPI]
		protected internal virtual void Initialize(PanelBase panelBase)
		{
			PanelBase = panelBase;
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = true;
			await UniTask.CompletedTask;
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = false;
			await UniTask.CompletedTask;
		}
	}
}
