using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dices;
using Dices.Rolling;

namespace Game.Models
{
	public class DiceContainer : IEnumerable<Dice>
	{
		private readonly Dice[] dicesRollers;

		public bool AnyDiceRolling => dicesRollers.Any(t => t.DiceRoller.State is DiceRollState.Rolling);
		public IEnumerable<Dice> Active => dicesRollers.Where(t => t.gameObject.activeSelf);

		public DiceContainer(Dice[] dicesRollers)
		{
			this.dicesRollers = dicesRollers;
		}

		public IEnumerator<Dice> GetEnumerator()
		{
			return dicesRollers.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}