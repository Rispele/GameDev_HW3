using System.Collections.Generic;
using Dices;

namespace Game.Services.ScoreCountStrategies
{
	public interface IScoresCountStrategy
	{
		public int Count(IEnumerable<Dice> dices);
	}
}