using QFramework;
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
			return target && self.IsFacingTarget(target.transform, dotThreshold);
		}

		public static float Distance(this Vector3 self, Vector3 target)
		{
			return Vector3.Distance(self, target);
		}

		public static bool IsInRange(this Transform self, Transform target, float range)
		{
			return target && self.position.Distance(target.position) <= range;
		}

		public static bool IsInRange(this Transform self, GameObject target, float range)
		{
			return target && self.position.Distance(target.Position()) <= range;
		}
	}
}