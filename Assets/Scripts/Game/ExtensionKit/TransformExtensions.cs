using UnityEngine;

namespace Game
{
	public static class TransformExtensions
	{
		public static bool IsFacingTarget(this Transform self, Transform target, float dotThreshold = 0.5f)
		{
			Vector3 direction = (target.position - self.position).normalized;
			float dot = Vector3.Dot(self.forward, direction);
			return dot >= dotThreshold;
		}

		public static bool IsFacingTarget(this Transform self, GameObject target, float dotThreshold = 0.5f)
		{
			return self.IsFacingTarget(target.transform, dotThreshold);
		}
	}
}