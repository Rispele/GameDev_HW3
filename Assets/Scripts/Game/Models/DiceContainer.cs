using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dices;
using Dices.Rolling;

namespace Game.Models
{
	public class DiceContainer : IEnumerable<Dice>
	{
		private readonly Dice[] dices;

		public IEnumerable<Dice> Alive => dices.Where(dice => dice);
		public bool AnyDiceRolling => Alive.Any(t => t.DiceRoller.State is DiceRollState.Rolling);
		public IEnumerable<Dice> Active => Alive.Where(t => t.gameObject.activeSelf);

		public DiceContainer(Dice[] dices)
		{
			this.dices = dices;
		}

		public IEnumerator<Dice> GetEnumerator()
		{
			return Alive.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}