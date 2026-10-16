using System;
using Game.Models;
using UnityEngine;

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