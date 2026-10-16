namespace Game.Models
{
	public enum DiceGameCycleState
	{
		None,
		Initiating,
		PreparedToRoll,
		Roll,
		InvalidRoll,
		CountScores,
		NoCombinations,
		Finish
	}
}