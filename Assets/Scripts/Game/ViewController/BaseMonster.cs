using DG.Tweening;
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
	public partial class BaseMonster : ViewController, IGameEndObserver, IGetHit
	{
		[SerializeField] private float _ViewRange;
		[SerializeField] private float _PatrolRange;
		[SerializeField] private float _LookAtTime;
		[SerializeField] private bool _IsGuard;

		protected GameObject _AttackTarget;
		private float _RemainingAttackCooldown;

		private bool _IsWalk;
		private bool _IsFollow;
		private bool _IsChase;
		private bool _IsDead;
		private bool _PlayerIsDead;
		private float _RemainLookAtTime;
		private EnemyState _State;
		private Vector3 _WayPoint;

		private Vector3 _InitPosition;
		private Quaternion _InitRotation;
		private float _InitSpeed;

		private Collider[] _Colliders = new Collider[10];
		private int _PlayerLayerMask;
		private string[] _LayerNames = new[] { "Player" };

		private void Awake()
		{
			_InitPosition = this.Position();
			_InitRotation = this.Rotation();
			_InitSpeed = SelfNavMeshAgent.speed;
			_RemainLookAtTime = _LookAtTime;
			_PlayerLayerMask = LayerMask.GetMask(_LayerNames);
		}

		private void Start()
		{
			SetInitialState(_IsGuard);
			GameManager.Instance.AddObserver(this);
		}

		private void Update()
		{
			DeathDetection();
			if (!_PlayerIsDead)
			{
				_RemainingAttackCooldown -= Time.deltaTime;
				SwitchStates();
				SetAnimationState();
			}
		}

		private void OnDisable() => GameManager.Instance.RemoveObserver(this);

		private void OnApplicationQuit() => gameObject.Hide();

		private void SetInitialState(bool isGuard)
		{
			if (isGuard)
			{
				_State = EnemyState.Guard;
			}
			else
			{
				_State = EnemyState.Patrol;
				GeneratePatrolPoint();
			}
		}

		private void DeathDetection()
		{
			if (SelfCharacterData.CurHealth > 0) return;
			_IsDead = true;
		}

		private void SetAnimationState()
		{
			SelfAnimator.SetBool(AnimatorHash.Walk, _IsWalk);
			SelfAnimator.SetBool(AnimatorHash.Follow, _IsFollow);
			SelfAnimator.SetBool(AnimatorHash.Chase, _IsChase);
			SelfAnimator.SetBool(AnimatorHash.Critical, SelfCharacterData.IsCritical);
			SelfAnimator.SetBool(AnimatorHash.Die, _IsDead);
		}

		private void SwitchStates()
		{
			if (_IsDead)
			{
				_State = EnemyState.Dead;
			}
			else if (IsPlayerInRange())
			{
				_State = EnemyState.Chase;
			}

			switch (_State)
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
			_IsFollow = false;
			if (this.Position() != _InitPosition)
			{
				_IsWalk = true;
				SelfNavMeshAgent.isStopped = false;
				SelfNavMeshAgent.destination = _InitPosition;

				if (this.Position().Distance(_InitPosition) <= SelfNavMeshAgent.stoppingDistance)
				{
					_IsWalk = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, _InitRotation, 0.01f);
				}
			}
		}

		private void Patrol()
		{
			_IsChase = false;
			_IsFollow = false;
			SelfNavMeshAgent.speed = _InitSpeed * 0.5f;
			if (this.Position().Distance(_WayPoint) <= SelfNavMeshAgent.stoppingDistance)
			{
				_IsWalk = false;
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
					_State = EnemyState.Guard;
				}
				else
				{
					_State = EnemyState.Patrol;
				}
			}
			else
			{
				SelfNavMeshAgent.destination = _AttackTarget.Position();
				_IsFollow = true;
				SelfNavMeshAgent.isStopped = false;
			}

			bool isTargetInAttackRange = transform.IsInRange(_AttackTarget, SelfCharacterData.AttackRange);
			bool isTargetInSkillRange = transform.IsInRange(_AttackTarget, SelfCharacterData.SkillRange);
			if (isTargetInAttackRange || isTargetInSkillRange)
			{
				_IsFollow = false;
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
			if (_RemainingAttackCooldown > 0) return;
			_RemainingAttackCooldown = SelfCharacterData.CoolDown;
			SelfCharacterData.IsCritical = Random.value <= SelfCharacterData.CriticalHitRate;
			transform.DOLookAt(_AttackTarget.Position(), 0.25f);
			if (isTargetInAttackRange)
			{
				SelfAnimator.SetTrigger(AnimatorHash.Attack);
			}
			else if (isTargetInSkillRange)
			{
				SelfAnimator.SetTrigger(AnimatorHash.Skill);
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

		private void GeneratePatrolPoint()
		{
			float randomX = Random.Range(-_PatrolRange, _PatrolRange);
			float randomZ = Random.Range(-_PatrolRange, _PatrolRange);
			var newPoint = new Vector3(_InitPosition.x + randomX, _InitPosition.y, _InitPosition.z + randomZ);
			_WayPoint = NavMesh.SamplePosition(newPoint, out NavMeshHit hit, _PatrolRange, 1)
				? hit.position
				: _InitPosition;
		}

		// Animation Event
		public void Hit()
		{
			if (transform.IsFacingTarget(_AttackTarget))
			{
				PlayerData.TakeHurt(SelfCharacterData, () => _AttackTarget.GetComponent<IGetHit>().GetHit());
			}
		}

		void IGameEndObserver.GameEndNotify()
		{
			_PlayerIsDead = true;
			_AttackTarget = null;
			_IsChase = false;
			_IsWalk = false;
			_IsFollow = false;
			SelfAnimator.SetBool(AnimatorHash.Win, true);
		}

		void IGetHit.GetHit()
		{
			SelfAnimator.SetTrigger(AnimatorHash.GetHit);
		}
	}
}