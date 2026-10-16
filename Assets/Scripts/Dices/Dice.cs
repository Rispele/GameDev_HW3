using System;
using System.Linq;
using Dices.Rolling;
using UnityEngine;

namespace Dices
{
	[RequireComponent(typeof(DiceRoller))]
	[RequireComponent(typeof(DiceHighlighter))]
	[RequireComponent(typeof(DiceSelect))]
	public class Dice : MonoBehaviour
	{
		private static readonly int[] OppositeFaces =
		{
			6,
			5,
			4,
			3,
			2,
			1
		};

		[SerializeField] private DiceFace[] diceFaces;
		[SerializeField] private DiceRoller diceRoller;
		[SerializeField] private DiceHighlighter diceHighlighter;
		[SerializeField] private DiceSelect diceSelect;

		public DiceRoller DiceRoller => diceRoller;
		public DiceSelect DiceSelect => diceSelect;

		public event Action<Dice, bool> OnSelected;

		public int? GetCurrentScore()
		{
			var triggeredFace = diceFaces.FirstOrDefault(t => t.IsCollisionEntered);

			return triggeredFace ? GetOppositeFace(triggeredFace.FaceNumber) : null;
		}

		private static int GetOppositeFace(int number)
		{
			return OppositeFaces[number - 1];
		}

		private void OnEnable()
		{
			diceSelect.OnGoingToSelect += SetHighlightedLevel1;
			diceSelect.OnSelected += SetHighlightedLevel2;
			diceSelect.OnSelected += DiceSelected;
		}

		private void OnDisable()
		{
			diceSelect.OnGoingToSelect -= SetHighlightedLevel1;
			diceSelect.OnSelected -= SetHighlightedLevel2;
			diceSelect.OnSelected -= DiceSelected;
		}

		private void SetHighlightedLevel1(bool on) => diceHighlighter.SetHighlighted(false, on);

		private void SetHighlightedLevel2(bool on) => diceHighlighter.SetHighlighted(on, diceSelect.IsGoingToSelect);

		private void DiceSelected(bool selected)
		{
			OnSelected?.Invoke(this, selected);
		}
	}
}