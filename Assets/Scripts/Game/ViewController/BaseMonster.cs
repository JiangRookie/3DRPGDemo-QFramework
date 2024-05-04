using System;
using QFramework;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public enum EnemyState { Guard, Patrol, Chase, Dead }

	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(CharacterData))]
	public partial class BaseMonster : ViewController, IEndGameObserver, IGetHit
	{
		private static readonly int s_Walk = Animator.StringToHash("Walk");
		private static readonly int s_Follow = Animator.StringToHash("Follow");
		private static readonly int s_Chase = Animator.StringToHash("Chase");
		private static readonly int s_Skill = Animator.StringToHash("Skill");
		private static readonly int s_Attack = Animator.StringToHash("Attack");
		private static readonly int s_Critical = Animator.StringToHash("Critical");
		private static readonly int s_Die = Animator.StringToHash("Die");
		private static readonly int s_Win = Animator.StringToHash("Win");
		private static readonly int s_GetHit = Animator.StringToHash("GetHit");
		[SerializeField] private float _ViewRange;
		[SerializeField] private float _PatrolRange;
		[SerializeField] private float _LookAtTime;
		[SerializeField] private bool _IsGuard;

		private GameObject _AttackTarget;
		private Collider[] _Colliders = new Collider[10];
		private EnemyState _EnemyState;
		private Vector3 _InitPosition;
		private Quaternion _InitRotation;
		private float _InitSpeed;
		private bool _IsChase;
		private bool _IsDead;
		private bool _IsFollow;
		private bool _IsWalk;
		private float _LastAttackTime;
		private string[] _LayerNames = new[] { "Player" };
		private bool _PlayerIsDead;
		private int _PlayerLayerMask;
		private float _RemainLookAtTime;
		private Vector3 _WayPoint;

		private void Awake()
		{
			_PlayerLayerMask = LayerMask.GetMask(_LayerNames);
			_InitSpeed = SelfNavMeshAgent.speed;
			_InitPosition = this.Position();
			_InitRotation = this.Rotation();
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
				GenerateRandomPatrolPoint();
			}
			GameManager.Instance.AddObserver(this);
		}

		private void Update()
		{
			if (SelfCharacterData.CurHealth <= 0)
			{
				_IsDead = true;
			}
			if (!_PlayerIsDead)
			{
				_LastAttackTime -= Time.deltaTime;
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
			SelfAnimator.SetBool(s_Win, true);
			_PlayerIsDead = true;
			_IsChase = false;
			_IsWalk = false;
			_AttackTarget = null;
		}

		public void GetHit()
		{
			SelfAnimator.SetTrigger(s_GetHit);
		}

		private void SetAnimationState()
		{
			SelfAnimator.SetBool(s_Walk, _IsWalk);
			SelfAnimator.SetBool(s_Follow, _IsFollow);
			SelfAnimator.SetBool(s_Chase, _IsChase);
			SelfAnimator.SetBool(s_Critical, SelfCharacterData.IsCritical);
			SelfAnimator.SetBool(s_Die, _IsDead);
		}

		private void SwitchStates()
		{
			if (_IsDead)
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
			_IsChase = false;
			if (this.Position() != _InitPosition)
			{
				_IsWalk = true;
				SelfNavMeshAgent.isStopped = false;
				SelfNavMeshAgent.destination = _InitPosition;

				if (Vector3.Distance(_InitPosition, this.Position()) <= SelfNavMeshAgent.stoppingDistance)
				{
					_IsWalk = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, _InitRotation, 0.01f);
				}
			}
		}

		private void Patrol()
		{
			_IsChase = false;
			SelfNavMeshAgent.speed = _InitSpeed * 0.5f;
			if (Vector3.Distance(this.Position(), _WayPoint) <= SelfNavMeshAgent.stoppingDistance)
			{
				_IsWalk = false;
				if (_RemainLookAtTime > 0)
				{
					_RemainLookAtTime -= Time.deltaTime;
				}
				else
				{
					_RemainLookAtTime = _LookAtTime;
					GenerateRandomPatrolPoint();
				}
			}
			else
			{
				_IsWalk = true;
				SelfNavMeshAgent.destination = _WayPoint;
			}
		}

		private void Chase()
		{
			_IsWalk = false;
			_IsChase = true;
			SelfNavMeshAgent.speed = _InitSpeed;
			if (!IsPlayerInRange())
			{
				_IsFollow = false;
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
				_IsFollow = true;
				SelfNavMeshAgent.isStopped = false;
			}

			if (IsTargetInAttackRange() || IsTargetInSkillRange())
			{
				_IsFollow = false;
				SelfNavMeshAgent.isStopped = true;
				AttackTarget();
			}
		}

		private void Dead()
		{
			SelfBoxCollider.enabled = false;
			SelfNavMeshAgent.enabled = false;
			Destroy(gameObject, 2f);
		}

		private void AttackTarget()
		{
			if (_LastAttackTime < 0)
			{
				_LastAttackTime = SelfCharacterData.CoolDown;
				SelfCharacterData.IsCritical = Random.value <= SelfCharacterData.CriticalHitRate;
				transform.LookAt(_AttackTarget.transform);
				if (IsTargetInAttackRange())
				{
					SelfAnimator.SetTrigger(s_Attack);
				}
				if (IsTargetInSkillRange())
				{
					SelfAnimator.SetTrigger(s_Skill);
				}
			}
		}

		private void GenerateRandomPatrolPoint()
		{
			float randomX = Random.Range(-_PatrolRange, _PatrolRange);
			float randomZ = Random.Range(-_PatrolRange, _PatrolRange);
			var newPoint = new Vector3(_InitPosition.x + randomX, this.Position().y, _InitPosition.z + randomZ);
			_WayPoint = NavMesh.SamplePosition(newPoint, out NavMeshHit hit, _PatrolRange, 1)
				? hit.position
				: this.Position();
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
			if (_AttackTarget)
			{
				TakeDamage(SelfCharacterData, () =>
				{
					_AttackTarget.GetComponent<IGetHit>().GetHit();
				});
			}
		}

		private void TakeDamage(CharacterData attacker, Action criticalAction)
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