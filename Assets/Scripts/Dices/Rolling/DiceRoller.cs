using System;
using Dices.Rolling.RollingStrategies;
using Extensions;
using UnityEngine;

namespace Dices.Rolling
{
	[RequireComponent(typeof(Rigidbody))]
	public class DiceRoller : MonoBehaviour
	{
		public IRollingStrategy RollingStrategy { get; set; }

		[field: SerializeField] public DiceRollState State { get; private set; } = DiceRollState.Rolled;
		[SerializeField] private new Rigidbody rigidbody = null!;

		public event Action<DiceRoller> Rolled;

		private void FixedUpdate()
		{
			var newState = rigidbody.IsMoving()
				? DiceRollState.Rolling
				: DiceRollState.Rolled;

			var eventToRaise = (newState, State) switch
			{
				(DiceRollState.Rolled, DiceRollState.Rolling) => Rolled,
				_ => null
			};

			State = newState;
			eventToRaise?.Invoke(this);
		}

		public void Roll()
		{
			var roleDecision = RollingStrategy.Roll(rigidbody);

			roleDecision.Apply(rigidbody);
			roleDecision.DrawForceRay();
		}
	}
}