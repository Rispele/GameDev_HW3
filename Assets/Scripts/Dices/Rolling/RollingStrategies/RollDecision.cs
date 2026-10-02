using UnityEngine;

namespace Dices.Rolling.RollingStrategies
{
	public record RollDecision(Vector3 ForcePosition, Vector3 Force)
	{
		public void Apply(Rigidbody rigidbody)
		{
			rigidbody.AddForceAtPosition(Force, ForcePosition, ForceMode.Impulse);
		}
	}
}