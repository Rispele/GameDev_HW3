using Game.InputControllers;
using Game.Models;
using Game.Services;
using Game.Services.ScoreCountStrategies;
using TMPro;
using UnityEngine;

namespace Game
{
	public class DiceGameBootstrapper : MonoBehaviour
	{
		[SerializeField] private DiceGameSettingsController diceSettingsController;
		[SerializeField] private DiceGameUIInputController inputController;

		[SerializeField] private TextMeshProUGUI totalScores;
		[SerializeField] private TextMeshProUGUI currentTurnScores;
		[SerializeField] private TextMeshProUGUI message;

		[SerializeField] private DiceGameSettings diceSettings;
		[SerializeField] private DicesSpawner dicesSpawner;

		private DiceContainer dices;
		private DicesRollController diceRollController;
		private DicePickController dicePickController;
		private DiceGameCycle gameCycle;

		private void OnEnable()
		{
			// dices = dicesSpawner.Dices;

			diceRollController = new DicesRollController(dices);
			dicePickController = new DicePickController(dices, new ZonkCountStrategy());
			gameCycle = new DiceGameCycle(
				dices,
				diceRollController,
				dicePickController);

			gameCycle.OnDiceGameStateChanged += inputController.OnDiceGameStateChanged;
			gameCycle.OnDiceGameStateChanged += diceSettings.OnDiceGameStateChanged;

			inputController.OnRoll += gameCycle.Roll;
			inputController.OnSubmitDice += OnDiceInputOnOnSubmitDice;
			inputController.OnRestart += OnDiceInputOnOnRestart;

			diceRollController.DicesRollSuccess += OnDiceRollControllerOnDicesRollSuccess;
			diceRollController.DicesRollFailure += OnDiceRollControllerOnDicesRollFailure;

			foreach (var diceRoller in dices)
			{
				// diceRoller.Dice.Clicked += dicePickController.PickDice;
			}


			gameCycle.TotalScoresChanged += OnGameCycleOnTotalScoresChanged;
			dicePickController.CurrentTurnScoresChanged += OnGameCycleOnCurrentTurnScoresChanged;
			gameCycle.OnDiceGameStateChanged += OnGameCycleOnOnDiceGameStateChanged;

			diceSettingsController.OnSettingsSubmitted += OnDiceSettingsControllerOnOnSettingsSubmitted;
		}

		private void OnDisable()
		{
			gameCycle.OnDiceGameStateChanged -= inputController.OnDiceGameStateChanged;
			gameCycle.OnDiceGameStateChanged -= diceSettings.OnDiceGameStateChanged;

			inputController.OnRoll -= gameCycle.Roll;
			inputController.OnSubmitDice -= OnDiceInputOnOnSubmitDice;
			inputController.OnRestart -= OnDiceInputOnOnRestart;

			diceRollController.DicesRollSuccess -= OnDiceRollControllerOnDicesRollSuccess;
			diceRollController.DicesRollFailure -= OnDiceRollControllerOnDicesRollFailure;

			foreach (var diceRoller in dices)
			{
				// diceRoller.Dice.Clicked -= dicePickController.PickDice;
			}

			gameCycle.TotalScoresChanged -= OnGameCycleOnTotalScoresChanged;
			dicePickController.CurrentTurnScoresChanged -= OnGameCycleOnCurrentTurnScoresChanged;
			gameCycle.OnDiceGameStateChanged -= OnGameCycleOnOnDiceGameStateChanged;

			diceRollController.Dispose();

			diceSettingsController.OnSettingsSubmitted -= OnDiceSettingsControllerOnOnSettingsSubmitted;

			gameCycle.Initialize();
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
				gameCycle.Initialize();
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
		}

		private void OnDiceRollControllerOnDicesRollFailure(DiceContainer _)
		{
			gameCycle.StopRolling(false);
		}

		private void OnDiceRollControllerOnDicesRollSuccess(DiceContainer _)
		{
			gameCycle.StopRolling(true);
		}

		private void OnDiceSettingsControllerOnOnSettingsSubmitted()
		{
			gameCycle.Start();
		}
	}
}