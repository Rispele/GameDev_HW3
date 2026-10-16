using System;
using System.Linq;
using Dices.Rolling.RollingStrategies;
using Game.Models;
using Game.Services.ScoreCountStrategies;
using JetBrains.Annotations;

namespace Game.Services
{
	public class DiceGameCycle
	{
		private readonly DiceGameSettings gameSettings;
		private readonly DicesSpawner spawner;
		private readonly IScoresCountStrategy countStrategy;

		[CanBeNull] private DiceContainer diceContainer;
		[CanBeNull] private DicesRollController rollController;
		[CanBeNull] private DicePickController pickController;


		private int totalScores;

		private int TotalScores
		{
			get => totalScores;
			set
			{
				if (totalScores == value)
				{
					return;
				}

				totalScores = value;
				TotalScoresChanged?.Invoke(totalScores);
			}
		}

		private int currentScores;

		private int CurrentScores
		{
			get => currentScores;
			set
			{
				if (currentScores == value)
				{
					return;
				}

				currentScores = value;
				CurrentScoresChanged?.Invoke(currentScores);
			}
		}

		public DiceGameCycleState State { get; private set; } = DiceGameCycleState.None;

		public DiceGameResult GameResult => TotalScores < gameSettings.ScoresToDraw
			? DiceGameResult.Lose
			: TotalScores < gameSettings.ScoresToWin
				? DiceGameResult.Draw
				: DiceGameResult.Win;

		public event Action<DiceGameCycleState> OnDiceGameStateChanged;
		public event Action<int> CurrentScoresChanged;
		public event Action<int> TotalScoresChanged;

		public DiceGameCycle(
			DiceGameSettings gameSettings,
			DicesSpawner spawner,
			IScoresCountStrategy countStrategy)
		{
			this.gameSettings = gameSettings;
			this.spawner = spawner;
			this.countStrategy = countStrategy;
		}

		public void Initialize() => WithStateTransition(() =>
			{
				if (diceContainer is not null)
				{
					spawner.Destroy(diceContainer);
					diceContainer = null;
				}

				TotalScores = 0;
				CurrentScores = 0;
				gameSettings.AllowEditing = true;
				gameSettings.Randomize();
			},
			DiceGameCycleState.Initiating,
			DiceGameCycleState.None, DiceGameCycleState.NoCombinations, DiceGameCycleState.Finish);

		public void Start() => WithStateTransition(() =>
			{
				gameSettings.AllowEditing = false;

				diceContainer = spawner.Spawn(gameSettings.DicesCount, new CenterDirectedRollingStrategy());
			},
			DiceGameCycleState.PreparedToRoll,
			DiceGameCycleState.Initiating);

		public void Roll() => WithStateTransition(() =>
			{
				if (diceContainer!.AnyDiceRolling)
				{
					return;
				}

				rollController = new DicesRollController(diceContainer);

				rollController.DicesRollFailure += StopRollingWithFailure;
				rollController.DicesRollSuccess += StopRollingWithSuccess;

				rollController.Roll();
			},
			DiceGameCycleState.Roll,
			DiceGameCycleState.PreparedToRoll, DiceGameCycleState.InvalidRoll);

		public void SubmitDices(bool finish) => WithStateValidation(() =>
			{
				if (!pickController!.Submit())
				{
					return;
				}

				pickController.Dispose();
				pickController = null;

				TotalScores += CurrentScores;
				CurrentScores = 0;

				if (finish)
				{
					ChangeState(DiceGameCycleState.Finish);
					return;
				}

				if (!diceContainer!.Active.Any())
				{
					GoToNextTurn();
				}
				else
				{
					ChangeState(DiceGameCycleState.PreparedToRoll);
				}
			},
			DiceGameCycleState.CountScores);

		private void StopRollingWithFailure() => WithStateTransition(() => { rollController?.Dispose(); },
			DiceGameCycleState.InvalidRoll,
			DiceGameCycleState.Roll);

		private void StopRollingWithSuccess() => WithStateValidation(() =>
			{
				rollController?.Dispose();
				rollController = null;

				if (!countStrategy.HasCombinations(diceContainer!.Active))
				{
					ChangeState(DiceGameCycleState.NoCombinations);
					return;
				}

				pickController = new DicePickController(diceContainer, countStrategy);
				pickController.ScoresChanged += OnCurrentScoresChanged;

				ChangeState(DiceGameCycleState.CountScores);
			},
			DiceGameCycleState.Roll);

		private void OnCurrentScoresChanged(int scores)
		{
			CurrentScores = scores;
		}

		private void WithStateTransition(Action action, DiceGameCycleState stateToGo, params DiceGameCycleState[] allowedStates)
		{
			WithStateValidation(action, allowedStates);
			ChangeState(stateToGo);
		}

		private void WithStateValidation(Action action, params DiceGameCycleState[] allowedStates)
		{
			if (!allowedStates.Contains(State))
			{
				return;
			}

			action();
		}

		private void GoToNextTurn()
		{
			foreach (var dice in diceContainer!)
			{
				dice.DiceSelect.ResetState();
				dice.gameObject.SetActive(true);
			}

			ChangeState(DiceGameCycleState.PreparedToRoll);
		}

		private void ChangeState(DiceGameCycleState newState)
		{
			State = newState;

			OnDiceGameStateChanged?.Invoke(State);
		}
	}
}