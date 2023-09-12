using System.Threading.Tasks;
using UnityEngine;

namespace ExtraTools.UI.Screen
{
	[RequireComponent(typeof(Canvas))]
	public abstract class ScreenUIBase : MonoBehaviour
	{
		[SerializeField] private Canvas canvas;

		protected ScreenBase ScreenBase;

		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			ScreenBase = screenBase;
		}

		protected internal virtual async Task Show()
		{
			canvas.enabled = true;
			await Task.CompletedTask;
		}

		protected internal virtual async Task Hide()
		{
			canvas.enabled = false;
			await Task.CompletedTask;
		}
	}
}