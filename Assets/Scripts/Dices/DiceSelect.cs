using System;
using UnityEngine;

namespace Dices
{
	public class DiceSelect : MonoBehaviour
	{
		[field: SerializeField] public bool AllowSelect { get; set; }
		public bool IsGoingToSelect { get; private set; }
		public bool IsSelected { get; private set; }

		public event Action<bool> OnGoingToSelect;
		public event Action<bool> OnSelected;

		public void ResetState()
		{
			AllowSelect = false;
			IsSelected = false;
			IsGoingToSelect = false;

			OnGoingToSelect?.Invoke(false);
			OnSelected?.Invoke(false);
		}

		private void OnMouseEnter()
		{
			if (IsSelected)
			{
				return;
			}

			IsGoingToSelect = true;
			OnGoingToSelect?.Invoke(IsGoingToSelect);
		}

		private void OnMouseDown()
		{
			if (!AllowSelect)
			{
				return;
			}

			IsSelected = !IsSelected;
			OnSelected?.Invoke(IsSelected);
		}

		private void OnMouseExit()
		{
			if (IsSelected)
			{
				return;
			}

			IsGoingToSelect = false;
			OnGoingToSelect?.Invoke(IsGoingToSelect);
		}
	}
}