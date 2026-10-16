using System;
using Game.Models;

namespace Game.InputControllers
{
	public interface IInputController
	{
		public event Action OnRoll;
		public event Action OnSubmitDice;
		public event Action OnRestart;

		public void OnGameCycleOnOnDiceGameStateChanged(DiceGameCycleState state);
	}
}