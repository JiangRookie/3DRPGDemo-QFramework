using UnityEngine;

namespace Game
{
	public enum EnemyState { Guard, Patrol, Chase, Dead }

	public class MonsterState
	{
		private static readonly int s_Walk = Animator.StringToHash("Walk");
		private static readonly int s_Follow = Animator.StringToHash("Follow");
		private static readonly int s_Chase = Animator.StringToHash("Chase");
		private static readonly int s_Die = Animator.StringToHash("Die");
		private static readonly int s_Critical = Animator.StringToHash("Critical");
		private static readonly int s_Win = Animator.StringToHash("Win");
		private static readonly int s_GetHit = Animator.StringToHash("GetHit");

		private CharacterData _SelfCharacterData;

		public MonsterState(float initSpeed, Vector3 initPosition, Quaternion initRotation, CharacterData characterData)
		{
			InitSpeed = initSpeed;
			InitPosition = initPosition;
			InitRotation = initRotation;
			_SelfCharacterData = characterData;
		}

		public float InitSpeed { get; }
		public Vector3 InitPosition { get; }
		public Quaternion InitRotation { get; }
		public bool IsChase { get; set; }
		public bool IsDead { get; set; }
		public bool IsFollow { get; set; }
		public bool IsWalk { get; set; }

		public void SetAnimationState(Animator selfAnimator)
		{
			selfAnimator.SetBool(s_Walk, IsWalk);
			selfAnimator.SetBool(s_Follow, IsFollow);
			selfAnimator.SetBool(s_Chase, IsChase);
			selfAnimator.SetBool(s_Critical, _SelfCharacterData.IsCritical);
			selfAnimator.SetBool(s_Die, IsDead);
		}

		public void SetWinState(Animator selfAnimator)
		{
			selfAnimator.SetBool(s_Win, true);
			IsChase = false;
			IsWalk = false;
		}

		public void SetGetHitState(Animator selfAnimator)
		{
			selfAnimator.SetTrigger(s_GetHit);
		}
	}
}