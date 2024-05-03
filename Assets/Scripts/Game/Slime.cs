using QFramework;
using UnityEngine;
using UnityEngine.AI;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public enum EnemyState { GUARD, PATROL, CHASE, DEAD }

	[RequireComponent(typeof(NavMeshAgent))]
	public partial class Slime : ViewController
	{
		private static readonly int s_Walk = Animator.StringToHash("Walk");
		private static readonly int s_Follow = Animator.StringToHash("Follow");
		private static readonly int s_Chase = Animator.StringToHash("Chase");
		[SerializeField] private EnemyState _EnemyState;
		[SerializeField] private float _ViewRange;
		[SerializeField] private float _PatrolRange;
		[SerializeField] private float _LookAtTime;
		[SerializeField] private bool _IsGuard;
		private GameObject _AttackTarget;
		private Collider[] _Colliders = new Collider[10];
		private Vector3 _InitPosition;
		private float _InitSpeed;
		private bool _IsChase;
		private bool _IsFollow;
		private bool _IsWalk;
		private string[] _LayerNames = new[] { "Player" };
		private int _PlayerLayerMask;
		private float _RemainLookAtTime;

		private Vector3 _WayPoint;

		private void Start()
		{
			_PlayerLayerMask = LayerMask.GetMask(_LayerNames);
			_InitSpeed = SelfNavMeshAgent.speed;
			_InitPosition = this.Position();
			_RemainLookAtTime = _LookAtTime;
			if (_IsGuard)
			{
				_EnemyState = EnemyState.GUARD;
			}
			else
			{
				_EnemyState = EnemyState.PATROL;
				GenerateRandomPatrolPoint();
			}
		}

		private void Update()
		{
			SwitchStates();
			SetAnimationState();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.Position(), _ViewRange);
			Gizmos.DrawWireSphere(this.Position(), _PatrolRange);
		}

		private void SetAnimationState()
		{
			SelfAnimator.SetBool(s_Walk, _IsWalk);
			SelfAnimator.SetBool(s_Follow, _IsFollow);
			SelfAnimator.SetBool(s_Chase, _IsChase);
		}

		private void SwitchStates()
		{
			if (IsPlayerInRange())
			{
				_EnemyState = EnemyState.CHASE;
			}

			switch (_EnemyState)
			{
				case EnemyState.GUARD:
					Guard();
					break;
				case EnemyState.PATROL:
					Patrol();
					break;
				case EnemyState.CHASE:
					Chase();
					break;
				case EnemyState.DEAD:
					Dead();
					break;
			}
		}

		public void Guard() { }

		public void Patrol()
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

		public void Chase()
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
					_EnemyState = EnemyState.GUARD;
				}
				else
				{
					_EnemyState = EnemyState.PATROL;
				}
			}
			else
			{
				SelfNavMeshAgent.destination = _AttackTarget.Position();
				_IsFollow = true;
			}
		}

		public void Dead() { }

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

		private void GenerateRandomPatrolPoint()
		{
			float randomX = Random.Range(-_PatrolRange, _PatrolRange);
			float randomZ = Random.Range(-_PatrolRange, _PatrolRange);
			var newPoint = new Vector3(_InitPosition.x + randomX, this.Position().y, _InitPosition.z + randomZ);
			_WayPoint = NavMesh.SamplePosition(newPoint, out NavMeshHit hit, _PatrolRange, 1)
				? hit.position
				: this.Position();
		}
	}
}