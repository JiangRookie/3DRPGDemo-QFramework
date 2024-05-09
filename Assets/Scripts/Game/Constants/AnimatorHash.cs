using UnityEngine;

namespace Game
{
	public static class AnimatorHash
	{
		public static readonly int Walk = Animator.StringToHash("Walk");
		public static readonly int Follow = Animator.StringToHash("Follow");
		public static readonly int Chase = Animator.StringToHash("Chase");
		public static readonly int Die = Animator.StringToHash("Die");
		public static readonly int Critical = Animator.StringToHash("Critical");
		public static readonly int Win = Animator.StringToHash("Win");
		public static readonly int GetHit = Animator.StringToHash("GetHit");
		public static readonly int Speed = Animator.StringToHash("Speed");
		public static readonly int Attack = Animator.StringToHash("Attack");
		public static readonly int Dizzy = Animator.StringToHash("Dizzy");
		public static readonly int Skill = Animator.StringToHash("Skill");
	}
}