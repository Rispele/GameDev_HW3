using System.Collections.Generic;
using Dices;

namespace Game.Services.ScoreCountStrategies
{
	public interface IScoresCountStrategy
	{
		public int Count(IEnumerable<Dice> dices);
		
		public bool HasCombinations(IEnumerable<Dice> dices);
		
		public bool IsValidPick(IEnumerable<Dice> dices);
	}
}