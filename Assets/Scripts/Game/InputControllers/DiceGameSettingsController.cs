using System;
using Game.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.InputControllers
{
	public class DiceGameSettingsController : MonoBehaviour
	{
		[SerializeField] private TMP_InputField diceCountInputField;
		[SerializeField] private TMP_InputField scoresToDrawInputField;
		[SerializeField] private TMP_InputField scoresToWinInputField;
		[SerializeField] private Button submitSettingsButton;

		[SerializeField] private DiceGameSettings diceGameSettings;

		public event Action OnSettingsSubmitted;

		private void OnEnable()
		{
			diceCountInputField.text = diceGameSettings.DicesCount.ToString();
			scoresToDrawInputField.text = diceGameSettings.ScoresToDraw.ToString();
			scoresToWinInputField.text = diceGameSettings.ScoresToWin.ToString();

			diceCountInputField.onEndEdit.AddListener(OnDiceCountEndEdit);
			scoresToDrawInputField.onEndEdit.AddListener(OnScoresToDrawEndEdit);
			scoresToWinInputField.onEndEdit.AddListener(OnScoresToWinEndEdit);

			diceGameSettings.OnSettingsChanged += OnSettingsChanged;
			diceGameSettings.OnAllowEditingChanged += OnAllowEditingChanged;

			submitSettingsButton.onClick.AddListener(SubmitSettings);
		}

		private void OnDisable()
		{
			diceCountInputField.onEndEdit.RemoveListener(OnDiceCountEndEdit);
			scoresToDrawInputField.onEndEdit.RemoveListener(OnScoresToDrawEndEdit);
			scoresToWinInputField.onEndEdit.RemoveListener(OnScoresToWinEndEdit);

			diceGameSettings.OnSettingsChanged -= OnSettingsChanged;
			diceGameSettings.OnAllowEditingChanged -= OnAllowEditingChanged;

			submitSettingsButton.onClick.RemoveListener(SubmitSettings);
		}

		private void OnDiceCountEndEdit(string text) => diceGameSettings.DicesCount = int.Parse(text);

		private void OnScoresToDrawEndEdit(string text) => diceGameSettings.ScoresToDraw = int.Parse(text);

		private void OnScoresToWinEndEdit(string text) => diceGameSettings.ScoresToWin = int.Parse(text);

		private void OnSettingsChanged(DiceGameSettingsSnapshot settings)
		{
			diceCountInputField.text = settings.DicesCount.ToString();
			scoresToDrawInputField.text = settings.ScoresToDraw.ToString();
			scoresToWinInputField.text = settings.ScoresToWin.ToString();
		}

		private void OnAllowEditingChanged(bool allow)
		{
			diceCountInputField.interactable = allow;
			scoresToDrawInputField.interactable = allow;
			scoresToWinInputField.interactable = allow;
			submitSettingsButton.interactable = allow;
		}

		private void SubmitSettings()
		{
			OnSettingsSubmitted?.Invoke();
		}
	}
}