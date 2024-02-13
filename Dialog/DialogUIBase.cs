using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraTools.UI.Dialog
{
	public abstract class DialogUIBase : MonoBehaviour
	{
		internal Action OnClicked;
		[SerializeField] protected Canvas _canvas;
		[SerializeField] private GameObject _buttonPrefab;
		[SerializeField] private TMP_Text _message;
		[SerializeField] private Transform _buttonsParent;

		private readonly List<DialogAnswer> _okAnswer = new() { new DialogAnswer("OK") };

		private readonly List<(Button, TMP_Text)> _buttons = new(2);

		protected virtual void OnDisable()
		{
			ClearButtons();
		}

		internal void Setup(string text, params DialogAnswer[] answers)
		{
			_message.text = text;

			if (answers != null)
			{
				SetupAnswers(answers);
			}
			else
			{
				SetupAnswers(_okAnswer);
			}
		}

		private void SetupAnswers(IList<DialogAnswer> answers)
		{
			if (_buttons.Count < answers.Count)
			{
				AddButtons(answers.Count - _buttons.Count);
			}

			for (int i = 0; i < answers.Count; i++)
			{
				_buttons[i].Item1.onClick.AddListener(Clicked);
				if (answers[i].Callback != null)
				{
					_buttons[i].Item1.onClick.AddListener(answers[i].Callback.Invoke);
				}

				_buttons[i].Item2.text = answers[i].Text;
				_buttons[i].Item1.gameObject.SetActive(true);
			}
		}

		private void AddButtons(int count)
		{
			for (int i = 0; i < count; i++)
			{
				GameObject button = Instantiate(_buttonPrefab, _buttonsParent);
				button.transform.localScale = Vector3.one;
				button.SetActive(false);
				_buttons.Add((button.GetComponent<Button>(), button.GetComponentInChildren<TMP_Text>()));
			}
		}

		private void Clicked()
		{
			OnClicked?.Invoke();
		}

		private void ClearButtons()
		{
			foreach ((Button button, TMP_Text _) in _buttons)
			{
				button.onClick.RemoveAllListeners();
				button.gameObject.SetActive(false);
			}
		}

		protected internal virtual async Task ShowAsync()
		{
			_canvas.enabled = true;
			await Task.CompletedTask;
		}

		protected internal virtual async Task HideAsync()
		{
			ClearButtons();
			_canvas.enabled = false;
			await Task.CompletedTask;
		}

#if UNITY_EDITOR
		[ContextMenu("Set Dialog UI")]
		internal void SetDialogUI()
		{
			_canvas = GetComponent<Canvas>();
		}
#endif
	}
}