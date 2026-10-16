using Game.Models;
using TMPro;
using UnityEngine;

namespace Game.InputControllers
{
	public class DiceGameSettingsController : MonoBehaviour
	{
		[SerializeField] private TMP_InputField diceCountInputField;
		[SerializeField] private TMP_InputField scoresToDrawInputField;
		[SerializeField] private TMP_InputField scoresToWinInputField;

		[SerializeField] private DiceGameSettings diceGameSettings;

		private void OnEnable()
		{
			diceCountInputField.onEndEdit.AddListener(OnDiceCountEndEdit);
			scoresToDrawInputField.onEndEdit.AddListener(OnScoresToDrawEndEdit);
			scoresToWinInputField.onEndEdit.AddListener(OnScoresToWinEndEdit);

			diceGameSettings.OnSettingsChanged += OnSettingsChanged;
			diceGameSettings.OnAllowEditingChanged += OnAllowEditingChanged;
		}

		private void OnDisable()
		{
			diceCountInputField.onEndEdit.RemoveListener(OnDiceCountEndEdit);
			scoresToDrawInputField.onEndEdit.RemoveListener(OnScoresToDrawEndEdit);
			scoresToWinInputField.onEndEdit.RemoveListener(OnScoresToWinEndEdit);

			diceGameSettings.OnSettingsChanged -= OnSettingsChanged;
			diceGameSettings.OnAllowEditingChanged -= OnAllowEditingChanged;
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
		}
	}
}