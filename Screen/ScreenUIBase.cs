using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace ExtraTools.UI.Screen
{
	[RequireComponent(typeof(Canvas))]
	public abstract class ScreenUIBase : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("_canvas")] protected Canvas Canvas;

		[PublicAPI] protected ScreenBase ScreenBase;

		[PublicAPI]
		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			ScreenBase = screenBase;
		}

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			Canvas.enabled = true;
			await UniTask.CompletedTask;
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			Canvas.enabled = false;
			await UniTask.CompletedTask;
		}
	}
}
