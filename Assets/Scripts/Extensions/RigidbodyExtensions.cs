using UnityEngine;

namespace Extensions
{
	public static class RigidbodyExtensions
	{
		public static bool IsMoving(this Rigidbody rigidbody)
		{
			return rigidbody.linearVelocity != Vector3.zero || rigidbody.angularVelocity != Vector3.zero;
		}
	}
}