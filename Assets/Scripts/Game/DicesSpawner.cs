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
		[SerializeField, Min(0.1f)] private float spawnRadius = 5f;

		private readonly List<DiceContainer> containersToDestroy = new();

		public DiceContainer Spawn(int diceCount, IRollingStrategy diceRollingStrategy)
		{
			var dices = new List<Dice>();
			for (var i = 0; i < diceCount; i++)
			{
				var angle = i * Mathf.PI * 2f / diceCount;
				var spawnPosition = new Vector3(Mathf.Cos(angle) * spawnRadius, 1f, Mathf.Sin(angle) * spawnRadius);
				var dice = Instantiate(dicePrefab, spawnPosition, Quaternion.identity);

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
