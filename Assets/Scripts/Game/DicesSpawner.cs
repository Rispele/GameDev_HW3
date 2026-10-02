using System.Linq;
using Dices.Rolling;
using Dices.Rolling.RollingStrategies;
using Game.Models;
using UnityEngine;

namespace Game
{
	public class DicesSpawner : MonoBehaviour
	{
		[SerializeField] private Transform diceParent;
		[SerializeField, Min(1)] private int diceCount;
		[SerializeField] private GameObject dicePrefab;

		public DiceContainer Dices { get; private set; }

		public void OnEnable()
		{
			if (Dices is not null)
			{
				return;
			}

			var dices = new DiceRoller[diceCount];

			for (var i = 0; i < diceCount; i++)
			{
				var dice = Instantiate(dicePrefab, new Vector3(i * 2, 1, i * 2), Quaternion.identity).GetComponent<DiceRoller>();

				dice.transform.SetParent(diceParent);
				dice.RollingStrategy = new CenterDirectedRollingStrategy();

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