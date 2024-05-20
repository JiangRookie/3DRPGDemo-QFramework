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
		[SerializeField] private float _ViewRange;
		[SerializeField] private float _PatrolRange;
		[SerializeField] private float _LookAtTime;
		[SerializeField] private bool _IsGuard;
		private float _AttackCooldown;
		protected GameObject _AttackTarget;
		private Collider[] _Colliders = new Collider[10];
		private Vector3 _InitPosition;
		private string[] _LayerNames = new[] { "Player" };
		private bool _PlayerIsDead;
		private int _PlayerLayerMask;
		private float _RemainLookAtTime;
		private Vector3 WayPoint { get; set; }
		private EnemyState State { get; set; }
		private float InitSpeed { get; set; }
		private Vector3 InitPosition { get; set; }
		private Quaternion InitRotation { get; set; }
		private bool IsChase { get; set; }
		private bool IsDead { get; set; }
		private bool IsFollow { get; set; }
		private bool IsWalk { get; set; }

		private void Awake()
		{
			InitPosition = transform.Position();
			InitRotation = transform.Rotation();
			InitSpeed = SelfNavMeshAgent.speed;
			_InitPosition = this.Position();
			_PlayerLayerMask = LayerMask.GetMask(_LayerNames);
			_RemainLookAtTime = _LookAtTime;
		}

		private void Start()
		{
			SetInitialState(_IsGuard);
			GameManager.Instance.AddObserver(this);
		}

		private void Update()
		{
			CheckAndSetDeadState();
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

		private void OnApplicationQuit()
		{
			gameObject.Hide();
		}

		public void EndNotify()
		{
			_PlayerIsDead = true;
			_AttackTarget = null;
			SetWinState();
		}

		public void GetHit()
		{
			SelfAnimator.SetTrigger(AnimatorHash.GetHit);
		}

		public void SetInitialState(bool isGuard)
		{
			if (isGuard)
			{
				State = EnemyState.Guard;
			}
			else
			{
				State = EnemyState.Patrol;
				GeneratePatrolPoint();
			}
		}

		public void CheckAndSetDeadState()
		{
			if (SelfCharacterData.CurHealth <= 0)
			{
				IsDead = true;
			}
		}

		public void SetAnimationState()
		{
			SelfAnimator.SetBool(AnimatorHash.Walk, IsWalk);
			SelfAnimator.SetBool(AnimatorHash.Follow, IsFollow);
			SelfAnimator.SetBool(AnimatorHash.Chase, IsChase);
			SelfAnimator.SetBool(AnimatorHash.Critical, SelfCharacterData.IsCritical);
			SelfAnimator.SetBool(AnimatorHash.Die, IsDead);
		}

		public void SetWinState()
		{
			SelfAnimator.SetBool(AnimatorHash.Win, true);
			IsChase = false;
			IsWalk = false;
		}

		private void SwitchStates()
		{
			if (IsDead)
			{
				State = EnemyState.Dead;
			}
			else if (IsPlayerInRange())
			{
				State = EnemyState.Chase;
			}

			switch (State)
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
			IsChase = false;
			if (this.Position() != InitPosition)
			{
				IsWalk = true;
				SelfNavMeshAgent.isStopped = false;
				SelfNavMeshAgent.destination = InitPosition;

				if (Vector3.Distance(InitPosition, this.Position()) <= SelfNavMeshAgent.stoppingDistance)
				{
					IsWalk = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, InitRotation, 0.01f);
				}
			}
		}

		private void Patrol()
		{
			IsChase = false;
			SelfNavMeshAgent.speed = InitSpeed * 0.5f;
			if (Vector3.Distance(this.Position(), WayPoint) <= SelfNavMeshAgent.stoppingDistance)
			{
				IsWalk = false;
				if (_RemainLookAtTime > 0)
				{
					_RemainLookAtTime -= Time.deltaTime;
				}
				else
				{
					_RemainLookAtTime = _LookAtTime;
					GeneratePatrolPoint();
				}
			}
			else
			{
				IsWalk = true;
				SelfNavMeshAgent.destination = WayPoint;
			}
		}

		private void Chase()
		{
			IsWalk = false;
			IsChase = true;
			SelfNavMeshAgent.speed = InitSpeed;
			if (!IsPlayerInRange())
			{
				IsFollow = false;
				if (_RemainLookAtTime > 0)
				{
					SelfNavMeshAgent.destination = this.Position();
					_RemainLookAtTime -= Time.deltaTime;
				}
				else if (_IsGuard)
				{
					State = EnemyState.Guard;
				}
				else
				{
					State = EnemyState.Patrol;
				}
			}
			else
			{
				SelfNavMeshAgent.destination = _AttackTarget.Position();
				IsFollow = true;
				SelfNavMeshAgent.isStopped = false;
			}

			bool isTargetInAttackRange = IsTargetInAttackRange();
			bool isTargetInSkillRange = IsTargetInSkillRange();
			if (isTargetInAttackRange || isTargetInSkillRange)
			{
				IsFollow = false;
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
				PlayerData.TakeHurt(SelfCharacterData, () => _AttackTarget.GetComponent<IGetHit>().GetHit());
			}
		}

		private void GeneratePatrolPoint()
		{
			float randomX = Random.Range(-_PatrolRange, _PatrolRange);
			float randomZ = Random.Range(-_PatrolRange, _PatrolRange);
			var newPoint = new Vector3(_InitPosition.x + randomX, _InitPosition.y, _InitPosition.z + randomZ);
			WayPoint = NavMesh.SamplePosition(newPoint, out NavMeshHit hit, _PatrolRange, 1)
				? hit.position
				: _InitPosition;
		}
	}
}