using System;
using System.Collections.Generic;
using System.Linq;
using Dices;
using Game.Models;
using Game.Services.ScoreCountStrategies;

namespace Game.Services
{
	public class DicePickController
	{
		private readonly DiceContainer dices;
		private readonly IScoresCountStrategy countStrategy;
		private readonly HashSet<Dice> pickedDices = new();

		private int currentStepScores;
		private int currentScores;

		public int CurrentScores
		{
			get => currentScores;
			private set
			{
				currentScores = value;
				CurrentTurnScoresChanged?.Invoke(CurrentScores);
			}
		}

		public event Action<int> CurrentTurnScoresChanged;

		public DicePickController(
			DiceContainer dices,
			IScoresCountStrategy countStrategy)
		{
			this.dices = dices;
			this.countStrategy = countStrategy;
		}

		public void PrepareForTurn()
		{
			CurrentScores = 0;
			currentStepScores = 0;

			pickedDices.Clear();
		}

		public void PrepareForStep()
		{
			currentStepScores = 0;

			pickedDices.Clear();

			foreach (var diceRoller in dices.Active)
			{
				diceRoller.Dice.AllowChoosing = true;
			}
		}

		public void PickDice(Dice dice)
		{
			if (dice.IsChosen)
			{
				pickedDices.Add(dice);
			}
			else
			{
				pickedDices.Remove(dice);
			}

			currentStepScores = countStrategy.Count(pickedDices);

			CurrentTurnScoresChanged?.Invoke(CurrentScores + currentStepScores);
		}

		public void Submit()
		{
			foreach (var diceRoller in dices.Active)
			{
				diceRoller.Dice.AllowChoosing = false;
			}

			foreach (var pickedDice in pickedDices)
			{
				pickedDice.gameObject.SetActive(false);
			}

			CurrentScores += currentStepScores;
		}

		public bool AnyCombinationOnTable()
		{
			return countStrategy.Count(dices.Active.Select(t => t.Dice)) > 0;
		}
	}
}