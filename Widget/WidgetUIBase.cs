using System.Threading;
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

		protected internal virtual async Task ShowAsync(WidgetTask task)
		{
			_text.text = task.Text;
			_canvas.enabled = true;
			CancellationTokenSource cts = new ();
			await Task.Delay((int)(1000 * task.ShowTime), cts.Token);
			_canvas.enabled = false;
		}
	}
}