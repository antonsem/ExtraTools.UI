using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ExtraTools.UI.Screen
{
	[RequireComponent(typeof(Canvas))]
	public abstract class ScreenUIBase : MonoBehaviour
	{
		[SerializeField] private Canvas _canvas;

		protected ScreenBase ScreenBase;

		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			ScreenBase = screenBase;
		}

		protected internal virtual async UniTask Show(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = true;
			await UniTask.CompletedTask;
		}

		protected internal virtual async UniTask Hide(CancellationToken cancellationToken = default)
		{
			_canvas.enabled = false;
			await UniTask.CompletedTask;
		}
	}
}
