using System.Collections.Generic;
using System.Linq;
using Dices;

namespace Game.Services.ScoreCountStrategies
{
	public class ZonkCountStrategy : IScoresCountStrategy
	{
		public int Count(IEnumerable<Dice> dices) => Evaluate(dices).Scores;

		public bool HasCombinations(IEnumerable<Dice> dices) => Evaluate(dices).HasCombinations;

		public bool IsValidPick(IEnumerable<Dice> dices) => Evaluate(dices).IsValidPick;

		private static ZonkScoreResult Evaluate(IEnumerable<Dice> dices)
		{
			return ZonkScoreCalculator.Evaluate(dices.Select(dice => dice.GetCurrentScore()!.Value));
		}
	}
}
