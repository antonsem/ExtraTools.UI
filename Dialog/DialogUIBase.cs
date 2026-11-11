using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtraTools.UI.Base;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ExtraTools.UI.Dialog
{
	[RequireComponent(typeof(Canvas))]
	public abstract class DialogUIBase : MonoBehaviour
	{
		internal Action OnClicked;
		[SerializeField, FormerlySerializedAs("_canvas")] protected Canvas Canvas;
		[SerializeField] private GameObject _buttonPrefab;
		[SerializeField] private TMP_Text _message;
		[SerializeField] private Transform _buttonsParent;

		private readonly List<DialogAnswer> _okAnswer = new() { new DialogAnswer("OK") };

		private readonly List<(Button, TMP_Text)> _buttons = new(2);

		private CanvasGroup _canvasGroup;

		// Only valid after Setup, which always activates at least one button.
		internal Selectable FirstAnswer => _buttons.Count > 0 ? _buttons[0].Item1 : null;

		[PublicAPI]
		protected virtual void OnDisable()
		{
			ClearButtons();
		}

		internal bool Contains(GameObject target)
		{
			return target.transform.IsChildOf(transform);
		}

		internal void SetInteractable(bool interactable)
		{
			Interactability.Set(this, ref _canvasGroup, interactable);
		}

		internal void Setup(string text, params DialogAnswer[] answers)
		{
			ClearButtons();
			_message.text = text;

			if (answers?.Length > 0)
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

		[PublicAPI]
		protected internal virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
		{
			Canvas.enabled = true;
			await UniTask.CompletedTask;
		}

		[PublicAPI]
		protected internal virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
		{
			ClearButtons();
			Canvas.enabled = false;
			await UniTask.CompletedTask;
		}
	}
}
