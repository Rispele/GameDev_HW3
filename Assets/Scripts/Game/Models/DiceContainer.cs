using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dices.Rolling;

namespace Game.Models
{
	public class DiceContainer : IEnumerable<DiceRoller>
	{
		private readonly DiceRoller[] dicesRollers;

		public bool AnyDiceRolling => dicesRollers.Any(t => t.State is DiceRollState.Rolling);
		public IEnumerable<DiceRoller> Active => dicesRollers.Where(t => t.gameObject.activeSelf);

		public DiceContainer(DiceRoller[] dicesRollers)
		{
			this.dicesRollers = dicesRollers;
		}

		public IEnumerator<DiceRoller> GetEnumerator()
		{
			return dicesRollers.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}