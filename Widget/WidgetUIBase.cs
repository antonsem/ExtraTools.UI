using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraTools.UI.Widget
{
	public abstract class WidgetUIBase : MonoBehaviour
	{
		[SerializeField] protected Canvas canvas;
		[SerializeField] protected TMP_Text text;
		[SerializeField] private Button button;

		protected WidgetBase Widget;

		protected virtual void OnEnable()
		{
			button.onClick.AddListener(OnClick);
		}

		protected virtual void OnDisable()
		{
			button.onClick.RemoveAllListeners();
		}

		protected virtual void OnClick()
		{
			Widget.OnClick();
		}

		protected internal virtual void Initialize(WidgetBase widget)
		{
			Widget = widget;
		}

		protected internal virtual async Task ShowAsync(WidgetTask task)
		{
			text.text = task.Text;
			canvas.enabled = true;
			await Task.Delay((int)(1000 * task.ShowTime));
			canvas.enabled = false;
		}
	}
}