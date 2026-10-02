using UnityEngine;

namespace Dices.Rolling.RollingStrategies
{
	public class CenterDirectedRollingStrategy : IRollingStrategy
	{
		public RollDecision Roll(Rigidbody rigidbody)
		{
			var forcePosition = GetForcePosition(rigidbody);
			var forceDirection = GetForceDirection(rigidbody);
			var forceStrength = rigidbody.mass * Random.Range(10, 15);

			return new RollDecision(forcePosition, Force: forceStrength * forceDirection);
		}

		private static Vector3 GetForceDirection(Rigidbody rigidbody)
		{
			var directionToCenterNormalized = (Vector3.zero - rigidbody.transform.position).normalized.Flatten();
			var flatBias = Random.insideUnitSphere.Flatten() * 1.5f;
			var upBias = Vector3.up * Random.Range(0, 1f);

			var directionVector = directionToCenterNormalized + flatBias + upBias;

			return directionVector.normalized;
		}

		private static Vector3 GetForcePosition(Rigidbody rigidbody)
		{
			return rigidbody.transform.TransformPoint(Random.insideUnitSphere);
		}
	}
}