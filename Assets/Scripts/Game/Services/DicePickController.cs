using System;
using System.Collections.Generic;
using Dices;
using Game.Models;
using Game.Services.ScoreCountStrategies;

namespace Game.Services
{
	public class DicePickController : IDisposable
	{
		private readonly DiceContainer dices;
		private readonly IScoresCountStrategy countStrategy;
		private readonly HashSet<Dice> pickedDices = new();

		public event Action<int> ScoresChanged;

		public DicePickController(
			DiceContainer dices,
			IScoresCountStrategy countStrategy)
		{
			this.dices = dices;
			this.countStrategy = countStrategy;

			SubscribeEvents();
		}

		public bool Submit()
		{
			if (!countStrategy.IsValidPick(pickedDices))
			{
				return false;
			}

			foreach (var dice in dices.Active)
			{
				dice.DiceSelect.AllowSelect = false;
			}

			foreach (var pickedDice in pickedDices)
			{
				pickedDice.gameObject.SetActive(false);
			}

			return true;
		}

		private void PickDice(Dice dice, bool selected)
		{
			if (selected)
			{
				pickedDices.Add(dice);
			}
			else
			{
				pickedDices.Remove(dice);
			}

			ScoresChanged?.Invoke(countStrategy.Count(pickedDices));
		}

		private void SubscribeEvents()
		{
			foreach (var dice in dices)
			{
				dice.DiceSelect.AllowSelect = true;
				dice.OnSelected += PickDice;
			}
		}

		public void Dispose()
		{
			foreach (var dice in dices)
			{
				dice.DiceSelect.AllowSelect = false;
				dice.OnSelected -= PickDice;
			}
		}
	}
}