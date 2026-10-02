using System;
using System.Linq;
using Dices.Rolling;
using Game.Models;

namespace Game.Services
{
	public class DicesRollController : IDisposable
	{
		private readonly DiceContainer diceContainer;

		public event Action<DiceContainer> DicesRollSuccess;
		public event Action<DiceContainer> DicesRollFailure;

		public DicesRollController(DiceContainer diceContainer)
		{
			this.diceContainer = diceContainer;

			SubscribeEvents();
		}

		public void Roll()
		{
			if (diceContainer.AnyDiceRolling)
			{
				return;
			}

			foreach (var diceRoller in diceContainer.Active)
			{
				diceRoller.Roll();
			}
		}

		private void DiceRolled(DiceRoller diceRoller)
		{
			if (diceContainer.AnyDiceRolling)
			{
				return;
			}

			if (diceContainer.Active.Select(t => t.Dice.GetCurrentScore()).Any(t => t is null))
			{
				DicesRollFailure?.Invoke(diceContainer);
			}
			else
			{
				DicesRollSuccess?.Invoke(diceContainer);
			}
		}

		private void SubscribeEvents()
		{
			foreach (var diceRoller in diceContainer)
			{
				diceRoller.Rolled += DiceRolled;
			}
		}

		public void Dispose()
		{
			foreach (var diceRoller in diceContainer)
			{
				diceRoller.Rolled -= DiceRolled;
			}
		}
	}
}