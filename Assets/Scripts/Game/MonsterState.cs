using QFramework;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
	public enum EnemyState { Guard, Patrol, Chase, Dead }

	public class MonsterState
	{
		private CharacterData _SelfCharacterData;
		private NavMeshAgent _SelfNavMeshAgent;

		public MonsterState(Transform transform, NavMeshAgent agent, CharacterData characterData)
		{
			InitPosition = transform.Position();
			InitRotation = transform.Rotation();
			_SelfNavMeshAgent = agent;
			InitSpeed = agent.speed;
			_SelfCharacterData = characterData;
		}

		public float InitSpeed { get; }
		public Vector3 InitPosition { get; }
		public Quaternion InitRotation { get; }
		public bool IsChase { get; set; }
		public bool IsDead { get; set; }
		public bool IsFollow { get; set; }
		public bool IsWalk { get; set; }

		public EnemyState State { get; set; }

		public void SetAnimationState(Animator selfAnimator)
		{
			selfAnimator.SetBool(AnimatorHash.Walk, IsWalk);
			selfAnimator.SetBool(AnimatorHash.Follow, IsFollow);
			selfAnimator.SetBool(AnimatorHash.Chase, IsChase);
			selfAnimator.SetBool(AnimatorHash.Critical, _SelfCharacterData.IsCritical);
			selfAnimator.SetBool(AnimatorHash.Die, IsDead);
		}

		public void SetWinState(Animator selfAnimator)
		{
			selfAnimator.SetBool(AnimatorHash.Win, true);
			IsChase = false;
			IsWalk = false;
		}

		public void SetGetHitState(Animator selfAnimator)
		{
			selfAnimator.SetTrigger(AnimatorHash.GetHit);
		}

		public void SetInitialState(bool isGuard, IPatrolPointGenerator patrolPointGenerator)
		{
			if (isGuard)
			{
				State = EnemyState.Guard;
			}
			else
			{
				State = EnemyState.Patrol;
				patrolPointGenerator.GeneratePatrolPoint();
			}
		}

		public void CheckAndSetDeadState()
		{
			if (_SelfCharacterData.CurHealth <= 0)
			{
				IsDead = true;
			}
		}
	}
}