using System.Collections.Generic;
using System.Linq;
using Dices;

namespace Game.Services.ScoreCountStrategies
{
	public class ZonkCountStrategy : IScoresCountStrategy
	{
		public int Count(IEnumerable<Dice> dices)
		{
			var countToNumbers = dices
				.Select(t => t.GetCurrentScore()!.Value)
				.GroupBy(t => t)
				.ToDictionary(g => g.Key, g => g.Count());

			var currentStepScores = 0;

			switch (countToNumbers.Count)
			{
				case 5 when !countToNumbers.ContainsKey(6):
					currentStepScores += 500;
					break;
				case 5 when !countToNumbers.ContainsKey(1):
					currentStepScores += 750;
					break;
				case 6:
					currentStepScores += 1000;
					break;
			}

			currentStepScores += CountRepeatedDices(countToNumbers, 1, 100, 1000);
			currentStepScores += CountRepeatedDices(countToNumbers, 2, 200);
			currentStepScores += CountRepeatedDices(countToNumbers, 3, 300);
			currentStepScores += CountRepeatedDices(countToNumbers, 4, 400);
			currentStepScores += CountRepeatedDices(countToNumbers, 5, 50, 500);
			currentStepScores += CountRepeatedDices(countToNumbers, 6, 600);

			return currentStepScores;
		}

		private int CountRepeatedDices(
			Dictionary<int, int> countToNumbers,
			int diceValue,
			int diceScoreForThreeOrMoreReps)
		{
			if (!countToNumbers.TryGetValue(diceValue, out var count2) || count2 is 1 or 2)
			{
				return 0;
			}

			return GetValueForRepeatedDices(diceScoreForThreeOrMoreReps, count2);
		}

		private int CountRepeatedDices(
			Dictionary<int, int> countToNumbers,
			int diceValue,
			int diceScore,
			int diceScoreForThreeOrMoreReps)
		{
			if (!countToNumbers.TryGetValue(diceValue, out var count))
			{
				return 0;
			}

			if (count is 1 or 2)
			{
				return diceScore * count;
			}

			return GetValueForRepeatedDices(diceScoreForThreeOrMoreReps, count);
		}

		private static int GetValueForRepeatedDices(int value, int count)
		{
			for (var i = 3; i < count; i++)
			{
				value *= 2;
			}

			return value;
		}
	}
}