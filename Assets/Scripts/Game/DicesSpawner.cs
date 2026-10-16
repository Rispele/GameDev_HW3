using System;
using System.Collections.Generic;
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

		private readonly List<DiceContainer> containersToDestroy = new();

		public DiceContainer Spawn(int diceCount, IRollingStrategy diceRollingStrategy)
		{
			if (diceCount is < 1 or > 6)
			{
				throw new ArgumentException("Dice count must be between 1 and 6");
			}

			var dices = new List<Dice>();
			for (var i = 0; i < diceCount; i++)
			{
				var dice = Instantiate(dicePrefab, new Vector3(i * 2, 1, i * 2), Quaternion.identity);

				dice.transform.SetParent(diceParent);
				dice.DiceRoller.RollingStrategy = diceRollingStrategy;

				dices.Add(dice);
			}

			var diceContainer = new DiceContainer(dices.ToArray());
			containersToDestroy.Add(diceContainer);

			return diceContainer;
		}

		private void OnDisable()
		{
			foreach (var dice in containersToDestroy.SelectMany(container => container.Where(dice => dice)))
			{
				Destroy(dice.gameObject);
			}
		}
	}
}