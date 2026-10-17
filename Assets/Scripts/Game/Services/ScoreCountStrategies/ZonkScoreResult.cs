namespace Game.Services.ScoreCountStrategies
{
	public readonly struct ZonkScoreResult
	{
		public int Scores { get; }
		public bool HasCombinations { get; }
		public bool IsValidPick => Scores > 0;

		internal ZonkScoreResult(int scores, bool hasCombinations)
		{
			Scores = scores;
			HasCombinations = hasCombinations;
		}
	}
}
