using System;
using Game.InputControllers;
using Game.Models;
using Game.Services;
using Game.Services.ScoreCountStrategies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class DiceGameBootstrapper : MonoBehaviour
	{
		[SerializeField] private MonoBehaviour inputController;
		[SerializeField] private DicesSpawner dicesSpawner;
		[SerializeField] private TextMeshProUGUI totalScores;
		[SerializeField] private TextMeshProUGUI currentTurnScores;
		[SerializeField] private TextMeshProUGUI message;
		[SerializeField] private Button rollButton;
		[SerializeField] private Button submitButton;
		[SerializeField] private Button finishButton;

		private IInputController diceInput;
		private DiceContainer dices;
		private DicesRollController diceRollController;
		private DicePickController dicePickController;
		private DiceGameCycle gameCycle;

		private void OnEnable()
		{
			diceInput = inputController as IInputController ?? throw new InvalidOperationException("Input controller is not an IInputController");
			dices = dicesSpawner.Dices;

			diceRollController = new DicesRollController(dices);
			dicePickController = new DicePickController(dices, new ZonkCountStrategy());
			gameCycle = new DiceGameCycle(
				dices,
				diceRollController,
				dicePickController);

			diceInput.OnRoll += gameCycle.Roll;
			diceRollController.DicesRollSuccess += OnDiceRollControllerOnDicesRollSuccess;
			diceRollController.DicesRollFailure += OnDiceRollControllerOnDicesRollFailure;

			foreach (var diceRoller in dices)
			{
				diceRoller.Dice.Clicked += dicePickController.PickDice;
			}

			diceInput.OnSubmitDice += OnDiceInputOnOnSubmitDice;
			diceInput.OnRestart += OnDiceInputOnOnRestart;

			gameCycle.TotalScoresChanged += OnGameCycleOnTotalScoresChanged;
			dicePickController.CurrentTurnScoresChanged += OnGameCycleOnCurrentTurnScoresChanged;
			gameCycle.OnDiceGameStateChanged += OnGameCycleOnOnDiceGameStateChanged;

			gameCycle.Start();
		}

		private void OnDisable()
		{
			diceInput.OnRoll -= gameCycle.Roll;
			diceRollController.DicesRollSuccess -= OnDiceRollControllerOnDicesRollSuccess;
			diceRollController.DicesRollFailure -= OnDiceRollControllerOnDicesRollFailure;

			foreach (var diceRoller in dices)
			{
				diceRoller.Dice.Clicked -= dicePickController.PickDice;
			}

			diceInput.OnSubmitDice -= OnDiceInputOnOnSubmitDice;
			diceInput.OnRestart -= OnDiceInputOnOnRestart;

			gameCycle.TotalScoresChanged -= OnGameCycleOnTotalScoresChanged;
			dicePickController.CurrentTurnScoresChanged -= OnGameCycleOnCurrentTurnScoresChanged;
			gameCycle.OnDiceGameStateChanged -= OnGameCycleOnOnDiceGameStateChanged;

			diceRollController.Dispose();
		}

		private void OnDiceInputOnOnSubmitDice()
		{
			gameCycle.SubmitDices(finish: false);
		}

		private void OnDiceInputOnOnRestart()
		{
			if (gameCycle.State is DiceGameCycleState.CountScores)
			{
				gameCycle.SubmitDices(finish: true);
			}
			else
			{
				gameCycle.Start();
			}
		}

		private void OnGameCycleOnTotalScoresChanged(int scores)
		{
			totalScores.text = $"Всего очков: {scores};";
		}

		private void OnGameCycleOnCurrentTurnScoresChanged(int scores)
		{
			currentTurnScores.text = $"Очков за этот ход: {scores};";
		}

		private void OnGameCycleOnOnDiceGameStateChanged(DiceGameCycleState state)
		{
			var messageToShow = state switch
			{
				DiceGameCycleState.PreparedToRoll => "Крути!",
				DiceGameCycleState.Roll => "Крутим...",
				DiceGameCycleState.CountScores => "Выбирай кости, которые хочешь зачесть на этом шаге!",
				DiceGameCycleState.NoCombinations => "Нет комбинаций, игра окончана!",
				DiceGameCycleState.InvalidRoll => "Крутка не пошла, попробуй ещё раз!",
				DiceGameCycleState.Finish => "Игра окончена!",
				_ => "..."
			};

			message.text = messageToShow;

			switch (state)
			{
				case DiceGameCycleState.None:
					break;
				case DiceGameCycleState.PreparedToRoll:
					rollButton.interactable = true;
					submitButton.interactable = false;
					finishButton.interactable = false;
					break;
				case DiceGameCycleState.Roll:
					rollButton.interactable = false;
					break;
				case DiceGameCycleState.InvalidRoll:
					rollButton.interactable = true;
					break;
				case DiceGameCycleState.CountScores:
					submitButton.interactable = true;
					finishButton.interactable = true;
					break;
				case DiceGameCycleState.NoCombinations:
				case DiceGameCycleState.Finish:
					finishButton.interactable = true;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		private void OnDiceRollControllerOnDicesRollFailure(DiceContainer _)
		{
			gameCycle.StopRolling(false);
		}

		private void OnDiceRollControllerOnDicesRollSuccess(DiceContainer _)
		{
			gameCycle.StopRolling(true);
		}
	}
}