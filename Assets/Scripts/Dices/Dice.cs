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
		public DiceHighlighter DiceHighlighter => diceHighlighter;
		public DiceSelect DiceSelect => diceSelect;

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
		}

		private void OnDisable()
		{
			diceSelect.OnGoingToSelect -= SetHighlightedLevel1;
			diceSelect.OnSelected -= SetHighlightedLevel2;
		}

		private void SetHighlightedLevel1(bool on) => diceHighlighter.SetHighlighted(false, on);

		private void SetHighlightedLevel2(bool on) => diceHighlighter.SetHighlighted(on, diceSelect.IsGoingToSelect);
	}
}