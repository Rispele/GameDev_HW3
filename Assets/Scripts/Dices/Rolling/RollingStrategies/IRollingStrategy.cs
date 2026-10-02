using UnityEngine;

namespace Dices.Rolling.RollingStrategies
{
	public interface IRollingStrategy
	{
		public RollDecision Roll(Rigidbody rigidbody);
	}
}