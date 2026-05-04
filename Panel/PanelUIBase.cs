using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ExtraTools.UI.Panel
{
	public abstract class PanelUIBase : MonoBehaviour
	{
		[SerializeField] protected Canvas _canvas;

		protected PanelBase PanelBase;

		protected internal virtual void Initialize(PanelBase panelBase)
		{
			PanelBase = panelBase;
		}

		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = true;
			await UniTask.CompletedTask;
		}

		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = false;
			await UniTask.CompletedTask;
		}
	}
}
