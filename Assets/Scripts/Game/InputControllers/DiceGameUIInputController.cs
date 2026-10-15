using System;
using UnityEngine;

namespace Game.InputControllers
{
	public class DiceGameUIInputController : MonoBehaviour, IInputController
	{
		public event Action OnRoll;
		public event Action OnSubmitDice;
		public event Action OnRestart;

		public void Roll()
		{
			OnRoll?.Invoke();
		}

		public void SubmitDice()
		{
			OnSubmitDice?.Invoke();
		}

		public void Restart()
		{
			OnRestart?.Invoke();
		}
	}
}