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
		[SerializeField] private TextMeshProUGUI currentScores;
		[SerializeField] private TextMeshProUGUI message;

		[SerializeField] private DiceGameSettings diceSettings;
		[SerializeField] private DicesSpawner dicesSpawner;

		private DiceGameCycle gameCycle;

		private void OnEnable()
		{
			gameCycle = new DiceGameCycle(diceSettings, dicesSpawner, new ZonkCountStrategy());

			gameCycle.OnDiceGameStateChanged += inputController.OnDiceGameStateChanged;
			gameCycle.OnDiceGameStateChanged += diceSettings.OnDiceGameStateChanged;
			gameCycle.OnDiceGameStateChanged += OnDiceGameStateChanged;

			inputController.OnRoll += gameCycle.Roll;
			inputController.OnSubmitDice += OnDiceInputOnOnSubmitDice;
			inputController.OnRestart += OnDiceInputOnOnRestart;

			gameCycle.TotalScoresChanged += OnTotalScoresChanged;
			gameCycle.CurrentScoresChanged += OnCurrentScoresChanged;

			diceSettingsController.OnSettingsSubmitted += OnSettingsSubmitted;

			gameCycle.Initialize();
		}

		private void OnDisable()
		{
			gameCycle.OnDiceGameStateChanged -= inputController.OnDiceGameStateChanged;
			gameCycle.OnDiceGameStateChanged -= diceSettings.OnDiceGameStateChanged;
			gameCycle.OnDiceGameStateChanged -= OnDiceGameStateChanged;

			inputController.OnRoll -= gameCycle.Roll;
			inputController.OnSubmitDice -= OnDiceInputOnOnSubmitDice;
			inputController.OnRestart -= OnDiceInputOnOnRestart;

			gameCycle.TotalScoresChanged -= OnTotalScoresChanged;
			gameCycle.CurrentScoresChanged -= OnCurrentScoresChanged;

			diceSettingsController.OnSettingsSubmitted -= OnSettingsSubmitted;
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

		private void OnCurrentScoresChanged(int scores)
		{
			currentScores.text = $"Выбрано на {scores} очков";
		}

		private void OnTotalScoresChanged(int scores)
		{
			totalScores.text = $"Всего очков: {scores}";
		}

		private void OnDiceGameStateChanged(DiceGameCycleState state)
		{
			var messageToShow = state switch
			{
				DiceGameCycleState.Initiating => "Настраиваем игру...",
				DiceGameCycleState.PreparedToRoll => "Крути!",
				DiceGameCycleState.Roll => "Крутим...",
				DiceGameCycleState.CountScores => "Выбирай кости, которые хочешь зачесть на этом шаге!",
				DiceGameCycleState.NoCombinations => "Нет комбинаций - поражение!",
				DiceGameCycleState.InvalidRoll => "Крутка не пошла, попробуй ещё раз!",
				DiceGameCycleState.Finish when gameCycle.GameResult is DiceGameResult.Lose => "Игра окончена - поражение!",
				DiceGameCycleState.Finish when gameCycle.GameResult is DiceGameResult.Draw => "Игра окончена - ничья!",
				DiceGameCycleState.Finish when gameCycle.GameResult is DiceGameResult.Win => "Игра окончена - победа!",
				_ => "..."
			};

			message.text = messageToShow;
		}

		private void OnSettingsSubmitted()
		{
			gameCycle.Start();
		}
	}
}