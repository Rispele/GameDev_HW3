using System.Collections.Generic;
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

		private readonly List<DiceContainer> containersToDestroy = new();

		public DiceContainer Spawn(int diceCount, IRollingStrategy diceRollingStrategy)
		{
			var dices = new List<Dice>();
			for (var i = 0; i < diceCount; i++)
			{
				var dice = Instantiate(dicePrefab, new Vector3(0, 1, 0), Quaternion.identity);

				dice.transform.SetParent(diceParent);
				dice.DiceRoller.RollingStrategy = diceRollingStrategy;

				dices.Add(dice);
			}

			var diceContainer = new DiceContainer(dices.ToArray());
			containersToDestroy.Add(diceContainer);

			return diceContainer;
		}

		public void Destroy(DiceContainer dices)
		{
			foreach (var dice in dices.Alive)
			{
				Destroy(dice.gameObject);
			}

			containersToDestroy.Remove(dices);
		}

		private void OnDisable()
		{
			foreach (var container in containersToDestroy)
			{
				Destroy(container);
			}
		}
	}
}