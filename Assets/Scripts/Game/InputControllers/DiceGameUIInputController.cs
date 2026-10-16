using System;
using Game.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Game.InputControllers
{
	public class DiceGameUIInputController : MonoBehaviour
	{
		[SerializeField] private Button rollButton;
		[SerializeField] private Button submitButton;
		[SerializeField] private Button finishButton;

		public event Action OnRoll;
		public event Action OnSubmitDice;
		public event Action OnRestart;

		public void OnDiceGameStateChanged(DiceGameCycleState state)
		{
			switch (state)
			{
				case DiceGameCycleState.None:
					break;
				case DiceGameCycleState.Initiating:
					rollButton.interactable = false;
					submitButton.interactable = false;
					finishButton.interactable = false;
					break;
				case DiceGameCycleState.PreparedToRoll:
					rollButton.interactable = true;
					submitButton.interactable = false;
					finishButton.interactable = false;
					break;
				case DiceGameCycleState.Roll:
					rollButton.interactable = false;
					submitButton.interactable = false;
					finishButton.interactable = false;
					break;
				case DiceGameCycleState.InvalidRoll:
					rollButton.interactable = true;
					submitButton.interactable = false;
					finishButton.interactable = false;
					break;
				case DiceGameCycleState.CountScores:
					rollButton.interactable = false;
					submitButton.interactable = true;
					finishButton.interactable = true;
					break;
				case DiceGameCycleState.NoCombinations:
				case DiceGameCycleState.Finish:
					rollButton.interactable = false;
					submitButton.interactable = false;
					finishButton.interactable = true;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		private void Roll()
		{
			OnRoll?.Invoke();
		}

		private void SubmitDice()
		{
			OnSubmitDice?.Invoke();
		}

		private void Restart()
		{
			OnRestart?.Invoke();
		}

		private void OnEnable()
		{
			rollButton.onClick.AddListener(Roll);
			submitButton.onClick.AddListener(SubmitDice);
			finishButton.onClick.AddListener(Restart);
		}

		private void OnDisable()
		{
			rollButton.onClick.RemoveListener(Roll);
			submitButton.onClick.RemoveListener(SubmitDice);
			finishButton.onClick.RemoveListener(Restart);
		}
	}
}