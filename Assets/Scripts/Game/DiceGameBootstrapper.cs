using Game.Models;
using Game.Services;
using Game.Services.ScoreCountStrategies;
using TMPro;
using UnityEngine;

namespace Game
{
	public class DiceGameBootstrapper : MonoBehaviour
	{
		[SerializeField] private DiceGameInputController diceInput;
		[SerializeField] private DicesSpawner dicesSpawner;
		[SerializeField] private TextMeshProUGUI totalScores;
		[SerializeField] private TextMeshProUGUI currentTurnScores;
		[SerializeField] private TextMeshProUGUI message;

		private DiceContainer dices;
		private DicesRollController diceRollController;
		private DicePickController dicePickController;
		private DiceGameCycle gameCycle;

		private void OnEnable()
		{
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
				DiceGameCycleState.PreparedToRoll => "Крути на пробел!",
				DiceGameCycleState.Roll => "Крутим...",
				DiceGameCycleState.CountScores =>
					"Выбирай кости, которые хочешь зачесть на этом шаге! (q - подтвердить завершить, e - подтвердить и продолжить)",
				DiceGameCycleState.NoCombinations => "Нет комбинаций, игра окончана! (Q - начать заново)",
				DiceGameCycleState.InvalidRoll => "Крутка не пошла, попробуй ещё раз!",
				DiceGameCycleState.Finish => "Игра окончена! (Q - начать заново)",
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
	}
}