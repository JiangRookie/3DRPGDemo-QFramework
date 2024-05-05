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
		private static readonly int s_Skill = Animator.StringToHash("Skill");
		private static readonly int s_Attack = Animator.StringToHash("Attack");
		[SerializeField] private float _ViewRange;
		[SerializeField] private float _PatrolRange;
		[SerializeField] private float _LookAtTime;
		[SerializeField] private bool _IsGuard;
		private float _AttackCooldown;
		protected GameObject _AttackTarget;
		private Collider[] _Colliders = new Collider[10];
		private EnemyState _EnemyState;
		private string[] _LayerNames = new[] { "Player" };
		private bool _PlayerIsDead;
		private int _PlayerLayerMask;
		private float _RemainLookAtTime;
		private Vector3 _WayPoint;
		private MonsterState MonsterState { get; set; }
		private PatrolPointGenerator PatrolPointGenerator { get; set; }

		private void Awake()
		{
			MonsterState = new MonsterState(
				SelfNavMeshAgent.speed,
				this.Position(),
				this.Rotation(),
				SelfCharacterData);
			PatrolPointGenerator = new PatrolPointGenerator(_PatrolRange, MonsterState.InitPosition);
			_PlayerLayerMask = LayerMask.GetMask(_LayerNames);
			_RemainLookAtTime = _LookAtTime;
		}

		private void Start()
		{
			if (_IsGuard)
			{
				_EnemyState = EnemyState.Guard;
			}
			else
			{
				_EnemyState = EnemyState.Patrol;
				_WayPoint = PatrolPointGenerator.GeneratePatrolPoint();
			}
			GameManager.Instance.AddObserver(this);
		}

		private void Update()
		{
			if (SelfCharacterData.CurHealth <= 0)
			{
				MonsterState.IsDead = true;
			}
			if (!_PlayerIsDead)
			{
				_AttackCooldown -= Time.deltaTime;
				SwitchStates();
				SetAnimationState();
			}
		}

		private void OnDisable()
		{
			GameManager.Instance.RemoveObserver(this);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.Position(), _ViewRange);
		}

		public void EndNotify()
		{
			_PlayerIsDead = true;
			_AttackTarget = null;
			MonsterState.SetWinState(SelfAnimator);
		}

		public void GetHit() => MonsterState.SetGetHitState(SelfAnimator);

		private void SetAnimationState() => MonsterState.SetAnimationState(SelfAnimator);

		private void SwitchStates()
		{
			if (MonsterState.IsDead)
			{
				_EnemyState = EnemyState.Dead;
			}
			else if (IsPlayerInRange())
			{
				_EnemyState = EnemyState.Chase;
			}

			switch (_EnemyState)
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
			if (Vector3.Distance(this.Position(), _WayPoint) <= SelfNavMeshAgent.stoppingDistance)
			{
				MonsterState.IsWalk = false;
				if (_RemainLookAtTime > 0)
				{
					_RemainLookAtTime -= Time.deltaTime;
				}
				else
				{
					_RemainLookAtTime = _LookAtTime;
					_WayPoint = PatrolPointGenerator.GeneratePatrolPoint();
				}
			}
			else
			{
				MonsterState.IsWalk = true;
				SelfNavMeshAgent.destination = _WayPoint;
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
					_EnemyState = EnemyState.Guard;
				}
				else
				{
					_EnemyState = EnemyState.Patrol;
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
					SelfAnimator.SetTrigger(s_Attack);
				}
				if (isTargetInSkillRange)
				{
					SelfAnimator.SetTrigger(s_Skill);
				}
			}
		}

		private bool IsPlayerInRange()
		{
			int numColliders = Physics.OverlapSphereNonAlloc(this.Position(), _ViewRange, _Colliders, _PlayerLayerMask);
			for (int i = 0; i < numColliders; i++)
			{
				if (_Colliders[i].CompareTag("Player"))
				{
					_AttackTarget = _Colliders[i].gameObject;
					return true;
				}
			}

			_AttackTarget = null;
			return false;
		}

		private bool IsTargetInAttackRange()
		{
			if (_AttackTarget)
			{
				return Vector3.Distance(_AttackTarget.Position(), this.Position()) <= SelfCharacterData.AttackRange + SelfNavMeshAgent.stoppingDistance;
			}
			return false;
		}

		private bool IsTargetInSkillRange()
		{
			if (_AttackTarget)
			{
				return Vector3.Distance(_AttackTarget.Position(), this.Position()) <= SelfCharacterData.SkillRange + SelfNavMeshAgent.stoppingDistance;
			}
			return false;
		}

		// Animation Event
		public void Hit()
		{
			if (_AttackTarget && transform.IsFacingTarget(_AttackTarget.transform))
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
			int realDamage = Mathf.Max((int)baseDamage - PlayerData.CurDefense.Value, 1);
			PlayerData.CurHealth.Value = Mathf.Max(PlayerData.CurHealth.Value - realDamage, 0);
		}
	}
}