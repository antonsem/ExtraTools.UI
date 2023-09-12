using System.Threading.Tasks;
using UnityEngine;

namespace ExtraTools.UI.Panel
{
	public abstract class PanelUIBase : MonoBehaviour
	{
		[SerializeField] protected Canvas canvas;

		protected PanelBase PanelBase;

		protected internal virtual void Initialize(PanelBase panelBase)
		{
			PanelBase = panelBase;
		}

		protected internal virtual async Task ShowAsync()
		{
			canvas.enabled = true;
			await Task.CompletedTask;
		}

		protected internal virtual async Task HideAsync()
		{
			canvas.enabled = false;
			await Task.CompletedTask;
		}
	}
}