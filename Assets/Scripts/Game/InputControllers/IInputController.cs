using System;

namespace Game.InputControllers
{
	public interface IInputController
	{
		public event Action OnRoll;
		public event Action OnSubmitDice;
		public event Action OnRestart;
	}
}