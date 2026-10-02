using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
	public class DiceGameInputController : MonoBehaviour
	{
		private DiceInput diceInput;

		public event Action OnRoll;
		public event Action OnSubmitDice;
		public event Action OnRestart;

		private void OnEnable()
		{
			if (diceInput is null)
			{
				diceInput = new DiceInput();
			}

			if (!diceInput.Dice.enabled)
			{
				diceInput.Dice.Enable();
			}

			diceInput.Dice.Roll.performed += OnRoleAdapter;
			diceInput.Dice.SubmitDices.performed += OnSubmitDiceAdapter;
			diceInput.Dice.Restart.performed += OnRestartAdapter;
		}

		private void OnDisable()
		{
			diceInput.Dice.Roll.performed -= OnRoleAdapter;
			diceInput.Dice.SubmitDices.performed -= OnSubmitDiceAdapter;
			diceInput.Dice.Restart.performed -= OnRestartAdapter;

			diceInput.Dice.Disable();
		}

		private void OnRoleAdapter(InputAction.CallbackContext _) => OnRoll?.Invoke();

		private void OnSubmitDiceAdapter(InputAction.CallbackContext _) => OnSubmitDice?.Invoke();

		private void OnRestartAdapter(InputAction.CallbackContext _) => OnRestart?.Invoke();
	}
}