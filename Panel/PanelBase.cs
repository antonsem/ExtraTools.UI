using System.Threading.Tasks;
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

		protected internal virtual async Task ShowAsync()
		{
			await _panelUI.ShowAsync();
		}

		protected internal virtual async Task HideAsync()
		{
			await _panelUI.HideAsync();
		}
	}
}
