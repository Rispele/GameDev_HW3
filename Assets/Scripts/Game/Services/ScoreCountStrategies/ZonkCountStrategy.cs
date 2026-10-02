using System.Collections.Generic;
using System.Linq;
using Dices;

namespace Game.Services.ScoreCountStrategies
{
	public class ZonkCountStrategy : IScoresCountStrategy
	{
		public int Count(IEnumerable<Dice> dices)
		{
			var leftDices = Count(dices, out var currentStepScores);

			return leftDices.Any() ? 0 : currentStepScores;
		}

		public bool HasCombinations(IEnumerable<Dice> dices)
		{
			Count(dices, out var currentStepScores);

			return currentStepScores > 0;
		}

		public bool IsValidPick(IEnumerable<Dice> dices)
		{
			var leftDices = Count(dices, out _);

			return !leftDices.Any();
		}

		private IEnumerable<int> Count(IEnumerable<Dice> dices, out int currentStepScores)
		{
			var countToNumbers = dices
				.Select(t => t.GetCurrentScore()!.Value)
				.GroupBy(t => t)
				.ToDictionary(g => g.Key, g => g.Count());

			currentStepScores = 0;

			if (HasKeys(countToNumbers, 1, 2, 3, 4, 5, 6))
			{
				currentStepScores += 1150;
				return Enumerable.Empty<int>();
			}
			
			if (HasKeys(countToNumbers, 1, 2, 3, 4, 5))
			{
				currentStepScores += 650;
				RemoveDices(countToNumbers, removeAll: false, 1, 2, 3, 4, 5);
			}

			if (HasKeys(countToNumbers, 2, 3, 4, 5, 6))
			{
				currentStepScores += 800;
				RemoveDices(countToNumbers, removeAll: false, 2, 3, 4, 5, 6);
			}

			currentStepScores += CountRepeatedDices(countToNumbers, 1, 100, 1000);
			currentStepScores += CountRepeatedDices(countToNumbers, 2, 200);
			currentStepScores += CountRepeatedDices(countToNumbers, 3, 300);
			currentStepScores += CountRepeatedDices(countToNumbers, 4, 400);
			currentStepScores += CountRepeatedDices(countToNumbers, 5, 50, 500);
			currentStepScores += CountRepeatedDices(countToNumbers, 6, 600);
			return countToNumbers.Keys;
		}

		private bool HasKeys(Dictionary<int, int> countToNumbers, params int[] keys)
		{
			return keys.All(countToNumbers.ContainsKey);
		}

		private void RemoveDices(Dictionary<int, int> countToNumbers, bool removeAll, params int[] keys)
		{
			foreach (var key in keys)
			{
				if (!countToNumbers.TryGetValue(key, out var count))
				{
					continue;
				}

				if (!removeAll && count > 1)
				{
					countToNumbers[key] = count - 1;
				}
				else
				{
					countToNumbers.Remove(key);
				}
			}
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

			RemoveDices(countToNumbers, removeAll: true, diceValue);

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

			RemoveDices(countToNumbers, removeAll: true, diceValue);

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