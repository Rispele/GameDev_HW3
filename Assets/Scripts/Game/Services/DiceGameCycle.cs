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

		public int TotalScores
		{
			get => totalScores;
			private set
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

		public void Start()
		{
			var isValidSate = CheckCurrentState(DiceGameCycleState.None)
			                  || CheckCurrentState(DiceGameCycleState.NoCombinations)
			                  || CheckCurrentState(DiceGameCycleState.Finish);

			if (!isValidSate)
			{
				return;
			}

			TotalScores = 0;
			pickController.PrepareForTurn();
			ResetAllDices();

			ChangeState(DiceGameCycleState.PreparedToRoll);
		}

		public void Roll()
		{
			if (!CheckCurrentState(DiceGameCycleState.PreparedToRoll) && !CheckCurrentState(DiceGameCycleState.InvalidRoll))
			{
				return;
			}

			rollController.Roll();

			ChangeState(DiceGameCycleState.Roll);
		}

		public void StopRolling(bool successfully)
		{
			if (!CheckCurrentState(DiceGameCycleState.Roll))
			{
				return;
			}

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
		}

		public void SubmitDices(bool finish)
		{
			if (!CheckCurrentState(DiceGameCycleState.CountScores))
			{
				return;
			}

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
				diceRoller.Dice.Reset();
				diceRoller.gameObject.SetActive(true);
			}
		}

		private bool CheckCurrentState(DiceGameCycleState stateToCheck)
		{
			return stateToCheck == State;
		}

		private void ChangeState(DiceGameCycleState newState)
		{
			State = newState;

			OnDiceGameStateChanged?.Invoke(State);
		}
	}
}