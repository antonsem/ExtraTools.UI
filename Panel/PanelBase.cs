using System.Threading.Tasks;
using ExtraTools.UI.Screen;
using UnityEngine;

namespace ExtraTools.UI.Panel
{
	public abstract class PanelBase : MonoBehaviour
	{
		[SerializeField] protected PanelUIBase panelUI;

		protected ScreenBase Screen;

		protected internal virtual void Initialize(ScreenBase screenBase)
		{
			Screen = screenBase;
			panelUI.Initialize(this);
		}

		protected internal virtual async Task ShowAsync()
		{
			await panelUI.ShowAsync();
		}

		protected internal virtual async Task HideAsync()
		{
			await panelUI.HideAsync();
		}
	}
}