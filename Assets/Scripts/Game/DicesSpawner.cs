using System.Linq;
using Dices;
using Dices.Rolling.RollingStrategies;
using Game.Models;
using UnityEngine;

namespace Game
{
	public class DicesSpawner : MonoBehaviour
	{
		[SerializeField] private Transform diceParent;
		[SerializeField] private Dice dicePrefab;

		[field: SerializeField, Range(1, 12)] public int DiceCount { get; set; } = 6;

		public DiceContainer Dices { get; private set; }
		
		public void OnEnable()
		{
			if (Dices is not null)
			{
				return;
			}

			var dices = new Dice[DiceCount];

			for (var i = 0; i < DiceCount; i++)
			{
				var dice = Instantiate(dicePrefab, new Vector3(i * 2, 1, i * 2), Quaternion.identity);

				dice.transform.SetParent(diceParent);
				dice.DiceRoller.RollingStrategy = new CenterDirectedRollingStrategy();

				dices[i] = dice;
			}

			Dices = new DiceContainer(dices);
		}

		private void OnDisable()
		{
			if (Dices == null)
			{
				return;
			}

			foreach (var dice in Dices.Where(t => t))
			{
				Destroy(dice.gameObject);
			}
		}
	}
}