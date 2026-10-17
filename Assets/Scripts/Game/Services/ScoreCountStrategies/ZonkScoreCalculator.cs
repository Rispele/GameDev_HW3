using System;
using System.Collections.Generic;

namespace Game.Services.ScoreCountStrategies
{
	public static class ZonkScoreCalculator
	{
		private static readonly (int FirstValue, int LastValue, int Scores)[] Straights =
		{
			(1, 6, 1150),
			(1, 5, 650),
			(2, 6, 800)
		};

		public static ZonkScoreResult Evaluate(IEnumerable<int> values)
		{
			var counts = new int[7];
			foreach (var value in values)
			{
				if (value < 1 || value > 6)
				{
					throw new ArgumentOutOfRangeException(nameof(values), "Dice values must be between 1 and 6.");
				}

				counts[value]++;
			}

			return CountStraights(counts, 0, 0);
		}

		private static ZonkScoreResult CountStraights(int[] counts, int straightIndex, int straightScores)
		{
			if (straightIndex == Straights.Length)
			{
				return CountRemainingDice(counts, straightScores);
			}

			var straight = Straights[straightIndex];
			var remaining = (int[])counts.Clone();
			var best = default(ZonkScoreResult);

			while (true)
			{
				var candidate = CountStraights(remaining, straightIndex + 1, straightScores);
				best = new ZonkScoreResult(
					Math.Max(best.Scores, candidate.Scores),
					best.HasCombinations || candidate.HasCombinations);

				if (!TryRemoveStraight(remaining, straight.FirstValue, straight.LastValue))
				{
					return best;
				}

				straightScores += straight.Scores;
			}
		}

		private static bool TryRemoveStraight(int[] counts, int firstValue, int lastValue)
		{
			for (var value = firstValue; value <= lastValue; value++)
			{
				if (counts[value] == 0)
				{
					return false;
				}
			}

			for (var value = firstValue; value <= lastValue; value++)
			{
				counts[value]--;
			}

			return true;
		}

		private static ZonkScoreResult CountRemainingDice(int[] counts, int straightScores)
		{
			var scores = straightScores;
			var hasUnscoredDice = false;

			for (var value = 1; value <= 6; value++)
			{
				var count = counts[value];
				if (count >= 3)
				{
					var tripleScores = value == 1 ? 1000 : value * 100;
					scores += tripleScores * (1 << (count - 3));
				}
				else
				{
					var singleScores = value == 1 ? 100 : value == 5 ? 50 : 0;
					scores += singleScores * count;
					hasUnscoredDice |= count > 0 && singleScores == 0;
				}
			}

			return new ZonkScoreResult(hasUnscoredDice ? 0 : scores, scores > 0);
		}
	}
}
