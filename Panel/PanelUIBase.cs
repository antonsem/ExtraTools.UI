using System.Threading.Tasks;
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

		protected internal virtual async Task ShowAsync()
		{
			_canvas.enabled = true;
			await Task.CompletedTask;
		}

		protected internal virtual async Task HideAsync()
		{
			_canvas.enabled = false;
			await Task.CompletedTask;
		}

#if UNITY_EDITOR
		internal void SetPanelUI()
		{
			_canvas = GetComponent<Canvas>();
		}
#endif
	}
}
