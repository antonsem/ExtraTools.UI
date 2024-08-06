using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraTools.UI.Widget
{
	public abstract class WidgetUIBase : MonoBehaviour
	{
		[SerializeField] protected Canvas _canvas;
		[SerializeField] protected TMP_Text _text;
		[SerializeField] private Button _button;

		protected WidgetBase Widget;

		protected virtual void OnEnable()
		{
			_button.onClick.AddListener(OnClick);
		}

		protected virtual void OnDisable()
		{
			_button.onClick.RemoveAllListeners();
		}

		protected virtual void OnClick()
		{
			Widget.OnClick();
		}

		protected internal virtual void Initialize(WidgetBase widget)
		{
			Widget = widget;
		}

		protected internal virtual Task ShowAsync(WidgetTask task)
		{
			_text.text = task.Text;
			_canvas.enabled = true;

			return Task.CompletedTask;
		}

		protected internal virtual Task HideAsync()
		{
			_canvas.enabled = false;
			return Task.CompletedTask;
		}
	}
}