using System;
using QFramework;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(CharacterData))]
	public partial class BaseMonster : ViewController, IEndGameObserver, IGetHit
	{
		[SerializeField] private float _ViewRange;
		[SerializeField] private float _PatrolRange;
		[SerializeField] private float _LookAtTime;
		[SerializeField] private bool _IsGuard;
		private float _AttackCooldown;
		protected GameObject _AttackTarget;
		private Collider[] _Colliders = new Collider[10];
		private string[] _LayerNames = new[] { "Player" };
		private bool _PlayerIsDead;
		private int _PlayerLayerMask;
		private float _RemainLookAtTime;
		private MonsterState MonsterState { get; set; }
		private IPatrolPointGenerator PatrolPointGenerator { get; set; }

		private void Awake()
		{
			MonsterState = new MonsterState(transform, SelfNavMeshAgent, SelfCharacterData);
			PatrolPointGenerator = new DefaultPatrolPointGenerator(_PatrolRange, this.Position());
			_PlayerLayerMask = LayerMask.GetMask(_LayerNames);
			_RemainLookAtTime = _LookAtTime;
		}

		private void Start()
		{
			MonsterState.SetInitialState(_IsGuard, PatrolPointGenerator);
			GameManager.Instance.AddObserver(this);
		}

		private void Update()
		{
			MonsterState.CheckAndSetDeadState();
			if (!_PlayerIsDead)
			{
				_AttackCooldown -= Time.deltaTime;
				SwitchStates();
				MonsterState.SetAnimationState(SelfAnimator);
			}
		}

		private void OnDisable()
		{
			GameManager.Instance.RemoveObserver(this);
		}

		private void OnApplicationQuit()
		{
			gameObject.Hide();
		}

		public void EndNotify()
		{
			_PlayerIsDead = true;
			_AttackTarget = null;
			MonsterState.SetWinState(SelfAnimator);
		}

		public void GetHit() => MonsterState.SetGetHitState(SelfAnimator);

		private void SwitchStates()
		{
			if (MonsterState.IsDead)
			{
				MonsterState.State = EnemyState.Dead;
			}
			else if (IsPlayerInRange())
			{
				MonsterState.State = EnemyState.Chase;
			}

			switch (MonsterState.State)
			{
				case EnemyState.Guard:
					Guard();
					break;
				case EnemyState.Patrol:
					Patrol();
					break;
				case EnemyState.Chase:
					Chase();
					break;
				case EnemyState.Dead:
					Dead();
					break;
			}
		}

		private void Guard()
		{
			MonsterState.IsChase = false;
			if (this.Position() != MonsterState.InitPosition)
			{
				MonsterState.IsWalk = true;
				SelfNavMeshAgent.isStopped = false;
				SelfNavMeshAgent.destination = MonsterState.InitPosition;

				if (Vector3.Distance(MonsterState.InitPosition, this.Position()) <= SelfNavMeshAgent.stoppingDistance)
				{
					MonsterState.IsWalk = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, MonsterState.InitRotation, 0.01f);
				}
			}
		}

		private void Patrol()
		{
			MonsterState.IsChase = false;
			SelfNavMeshAgent.speed = MonsterState.InitSpeed * 0.5f;
			if (Vector3.Distance(this.Position(), PatrolPointGenerator.WayPoint) <= SelfNavMeshAgent.stoppingDistance)
			{
				MonsterState.IsWalk = false;
				if (_RemainLookAtTime > 0)
				{
					_RemainLookAtTime -= Time.deltaTime;
				}
				else
				{
					_RemainLookAtTime = _LookAtTime;
					PatrolPointGenerator.GeneratePatrolPoint();
				}
			}
			else
			{
				MonsterState.IsWalk = true;
				SelfNavMeshAgent.destination = PatrolPointGenerator.WayPoint;
			}
		}

		private void Chase()
		{
			MonsterState.IsWalk = false;
			MonsterState.IsChase = true;
			SelfNavMeshAgent.speed = MonsterState.InitSpeed;
			if (!IsPlayerInRange())
			{
				MonsterState.IsFollow = false;
				if (_RemainLookAtTime > 0)
				{
					SelfNavMeshAgent.destination = this.Position();
					_RemainLookAtTime -= Time.deltaTime;
				}
				else if (_IsGuard)
				{
					MonsterState.State = EnemyState.Guard;
				}
				else
				{
					MonsterState.State = EnemyState.Patrol;
				}
			}
			else
			{
				SelfNavMeshAgent.destination = _AttackTarget.Position();
				MonsterState.IsFollow = true;
				SelfNavMeshAgent.isStopped = false;
			}

			bool isTargetInAttackRange = IsTargetInAttackRange();
			bool isTargetInSkillRange = IsTargetInSkillRange();
			if (isTargetInAttackRange || isTargetInSkillRange)
			{
				MonsterState.IsFollow = false;
				SelfNavMeshAgent.isStopped = true;
				AttackTarget(isTargetInAttackRange, isTargetInSkillRange);
			}
		}

		private void Dead()
		{
			SelfBoxCollider.enabled = false;
			SelfNavMeshAgent.radius = 0;
			Destroy(gameObject, 2f);
		}

		private void AttackTarget(bool isTargetInAttackRange, bool isTargetInSkillRange)
		{
			if (_AttackCooldown < 0)
			{
				_AttackCooldown = SelfCharacterData.CoolDown;
				SelfCharacterData.IsCritical = Random.value <= SelfCharacterData.CriticalHitRate;
				transform.LookAt(_AttackTarget.transform);
				if (isTargetInAttackRange)
				{
					SelfAnimator.SetTrigger(AnimatorHash.Attack);
				}
				if (isTargetInSkillRange)
				{
					SelfAnimator.SetTrigger(AnimatorHash.Skill);
				}
			}
		}

		private bool IsPlayerInRange()
		{
			int numColliders = Physics.OverlapSphereNonAlloc(this.Position(), _ViewRange, _Colliders, _PlayerLayerMask);
			for (int i = 0; i < numColliders; i++)
			{
				if (!_Colliders[i].CompareTag("Player")) continue;
				_AttackTarget = _Colliders[i].gameObject;
				return true;
			}

			_AttackTarget = null;
			return false;
		}

		private bool IsTargetInAttackRange() =>
			_AttackTarget && _AttackTarget.Distance(this) <= SelfCharacterData.AttackRange;

		private bool IsTargetInSkillRange() =>
			_AttackTarget && _AttackTarget.Distance(this) <= SelfCharacterData.SkillRange;

		// Animation Event
		public void Hit()
		{
			if (_AttackTarget && transform.IsFacingTarget(_AttackTarget))
			{
				TakeDamage(SelfCharacterData, () =>
				{
					_AttackTarget.GetComponent<IGetHit>().GetHit();
				});
			}
		}

		protected void TakeDamage(CharacterData attacker, Action criticalAction = null)
		{
			float baseDamage = Random.Range(attacker.MinDamage, attacker.MaxDamage + 1);
			if (attacker.IsCritical)
			{
				baseDamage *= attacker.CriticalHitBonusPercentage;
				criticalAction?.Invoke();
			}
			PlayerData.TakeHurt((int)baseDamage);
		}
	}
}