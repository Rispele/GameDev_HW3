namespace Game.Models
{
	public enum DiceGameCycleState
	{
		None,
		PreparedToRoll,
		Roll,
		InvalidRoll,
		CountScores,
		NoCombinations,
		Finish
	}
}