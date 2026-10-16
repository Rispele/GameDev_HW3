using System;
using System.Linq;
using Game.Models;

namespace Game.Services
{
	public class DiceGameCycle
	{
		private readonly DiceContainer dices;
		private readonly DicesRollController rollController;
		private readonly DicePickController pickController;

		public DiceGameCycleState State { get; private set; } = DiceGameCycleState.None;
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

		public event Action<DiceGameCycleState> OnDiceGameStateChanged;
		public event Action<int> TotalScoresChanged;

		public DiceGameCycle(
			DiceContainer dices,
			DicesRollController rollController,
			DicePickController pickController)
		{
			this.dices = dices;
			this.rollController = rollController;
			this.pickController = pickController;
		}

		public void Start() => WithStateTransition(() =>
			{
				TotalScores = 0;
				pickController.PrepareForTurn();
				ResetAllDices();
			},
			DiceGameCycleState.PreparedToRoll,
			DiceGameCycleState.None, DiceGameCycleState.NoCombinations, DiceGameCycleState.Finish);

		public void Roll() => WithStateTransition(() => { rollController.Roll(); },
			DiceGameCycleState.Roll,
			DiceGameCycleState.PreparedToRoll, DiceGameCycleState.InvalidRoll);

		public void StopRolling(bool successfully) => WithStateValidation(() =>
			{
				if (!successfully)
				{
					ChangeState(DiceGameCycleState.InvalidRoll);
					return;
				}

				if (!pickController.AnyCombinationOnTable())
				{
					ChangeState(DiceGameCycleState.NoCombinations);
					return;
				}

				pickController.PrepareForStep();
				ChangeState(DiceGameCycleState.CountScores);
			},
			DiceGameCycleState.Roll);

		public void SubmitDices(bool finish) => WithStateValidation(() =>
			{
				if (!pickController.Submit())
				{
					return;
				}

				if (finish)
				{
					TotalScores += pickController.CurrentScores;
					ChangeState(DiceGameCycleState.Finish);
					return;
				}

				if (!dices.Active.Any())
				{
					TotalScores += pickController.CurrentScores;

					GoToNextTurn();
				}
				else
				{
					ChangeState(DiceGameCycleState.PreparedToRoll);
				}
			},
			DiceGameCycleState.CountScores);

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
			pickController.PrepareForTurn();
			ResetAllDices();

			ChangeState(DiceGameCycleState.PreparedToRoll);
		}

		private void ResetAllDices()
		{
			foreach (var diceRoller in dices.Where(t => !t.gameObject.activeSelf))
			{
				// diceRoller.Dice.Reset();
				diceRoller.gameObject.SetActive(true);
			}
		}

		private void ChangeState(DiceGameCycleState newState)
		{
			State = newState;

			OnDiceGameStateChanged?.Invoke(State);
		}
	}
}