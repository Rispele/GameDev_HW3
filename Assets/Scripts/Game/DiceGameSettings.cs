using System;
using Game.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
	public class DiceGameSettings : MonoBehaviour
	{
		private const int MinDicesCount = 3;
		private const int MaxDicesCount = 12;

		[SerializeField] private bool allowEditing = true;
		[SerializeField] private int dicesCount;
		[SerializeField] private int scoresToDraw;
		[SerializeField] private int scoresToWin;

		public bool AllowEditing
		{
			get => allowEditing;
			set
			{
				if (allowEditing == value)
				{
					return;
				}

				allowEditing = value;
				OnAllowEditingChanged?.Invoke(allowEditing);
			}
		}

		public int DicesCount
		{
			get => dicesCount;
			set => WithEditingHooks(() =>
			{
				if (dicesCount == value)
				{
					return;
				}

				dicesCount = value switch
				{
					< MinDicesCount => MinDicesCount,
					> MaxDicesCount => MaxDicesCount,
					_ => value
				};
			});
		}

		public int ScoresToDraw
		{
			get => scoresToDraw;
			set => WithEditingHooks(() => scoresToDraw = value);
		}

		public int ScoresToWin
		{
			get => scoresToWin;
			set => WithEditingHooks(() => scoresToWin = value);
		}

		public event Action<DiceGameSettingsSnapshot> OnSettingsChanged;
		public event Action<bool> OnAllowEditingChanged;

		public void OnDiceGameStateChanged(DiceGameCycleState state)
		{
			AllowEditing = state switch
			{
				DiceGameCycleState.Initiating => true,
				DiceGameCycleState.PreparedToRoll
					or DiceGameCycleState.Roll
					or DiceGameCycleState.InvalidRoll
					or DiceGameCycleState.CountScores
					or DiceGameCycleState.NoCombinations
					or DiceGameCycleState.Finish => false,
				_ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
			};
		}

		public void Randomize()
		{
			DicesCount = Random.Range(MinDicesCount, MaxDicesCount);

			var factor1 = Math.Min(DicesCount, 6) - 4.5;
			var factor2 = DicesCount / 6.0;

			ScoresToDraw = (int)(Math.Pow(2, factor1) * Random.Range(300, 450) * factor2);
			ScoresToWin = (int)(Math.Pow(2, factor1) * Random.Range(500, 650) * factor2);
		}

		private void WithEditingHooks(Action action)
		{
			if (!AllowEditing)
			{
				return;
			}

			action();
			OnSettingsChanged?.Invoke(new DiceGameSettingsSnapshot(dicesCount, scoresToDraw, scoresToWin));
		}
	}
}