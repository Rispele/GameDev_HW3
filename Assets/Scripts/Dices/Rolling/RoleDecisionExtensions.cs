using Dices.Rolling.RollingStrategies;
using UnityEngine;

namespace Dices.Rolling
{
	public static class RoleDecisionExtensions
	{
		public static void DrawForceRay(this RollDecision decision)
		{
			Debug.DrawRay(
				start: decision.ForcePosition, 
				dir: decision.Force, 
				color: Color.red, 
				duration: 3);
		}
	}
}