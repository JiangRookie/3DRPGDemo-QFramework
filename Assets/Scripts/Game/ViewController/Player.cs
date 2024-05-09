using System.Collections;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public partial class Player : ViewController, IGetHit, IPushable
	{
		private float _AttackCooldown;
		private GameObject _AttackTarget;
		private bool _IsDead;
		private float _StopDistance;

		private void Awake()
		{
			_StopDistance = SelfNavMeshAgent.stoppingDistance;
		}

		private void Start()
		{
			MouseManager.OnMouseClicked
			   .Register(MoveToTarget)
			   .UnRegisterWhenGameObjectDestroyed(gameObject);

			MouseManager.OnEnemyClicked
			   .Register(MoveToAttackTarget)
			   .UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		private void Update()
		{
			if (PlayerData.CurHealth.Value <= 0)
			{
				_IsDead = true;
				GameManager.Instance.NotifyObservers();
			}
			_AttackCooldown -= Time.deltaTime;
			SwitchAnimation();
		}

		public void GetHit()
		{
			SelfAnimator.SetTrigger(AnimatorHash.GetHit);
		}

		public void SetPushed(Vector3 pushedToPosition)
		{
			SelfNavMeshAgent.isStopped = true;
			SelfNavMeshAgent.velocity = pushedToPosition;
			SelfAnimator.SetTrigger(AnimatorHash.Dizzy);
		}

		private void SwitchAnimation()
		{
			SelfAnimator.SetFloat(AnimatorHash.Speed, SelfNavMeshAgent.velocity.sqrMagnitude);
			SelfAnimator.SetBool(AnimatorHash.Die, _IsDead);
		}

		private void MoveToTarget(Vector3 targetPoint)
		{
			StopAllCoroutines();
			if (_IsDead) return;
			SelfNavMeshAgent.stoppingDistance = _StopDistance;
			SelfNavMeshAgent.isStopped = false;
			SelfNavMeshAgent.destination = targetPoint;
		}

		private void MoveToAttackTarget(GameObject targetGameObj)
		{
			if (_IsDead) return;
			if (targetGameObj)
			{
				_AttackTarget = targetGameObj;
				StartCoroutine(MoveToAttackTargetCoroutine());
			}
		}

		private IEnumerator MoveToAttackTargetCoroutine()
		{
			SelfNavMeshAgent.isStopped = false;
			SelfNavMeshAgent.stoppingDistance = PlayerData.AttackRange.Value;

			transform.LookAt(_AttackTarget.transform);

			while (Vector3.Distance(_AttackTarget.Position(), this.Position()) > PlayerData.AttackRange.Value)
			{
				SelfNavMeshAgent.destination = _AttackTarget.Position();
				yield return null;
			}

			SelfNavMeshAgent.isStopped = true;

			AttackTarget();
		}

		private void AttackTarget()
		{
			if (_AttackCooldown < 0)
			{
				PlayerData.IsCritical.Value = Random.value <= PlayerData.CriticalHitRate.Value;
				SelfAnimator.SetBool(AnimatorHash.Critical, PlayerData.IsCritical.Value);
				SelfAnimator.SetTrigger(AnimatorHash.Attack);
				_AttackCooldown = PlayerData.CoolDown.Value;
			}
		}

		public void Hit()
		{
			if (_AttackTarget)
			{
				if (_AttackTarget.CompareTag("Attackable"))
				{
					var rock = _AttackTarget.GetComponent<Rock>();
					if (rock && rock.RockState == RockState.HitNothing)
					{
						rock.HandleHit(transform.forward);
					}
				}
				else if (_AttackTarget.CompareTag("Enemy"))
				{
					PlayerData.TakeDamage(_AttackTarget.GetComponent<CharacterData>(),
						() =>
						{
							_AttackTarget.GetComponent<IGetHit>().GetHit();
						});
				}
			}
		}
	}
}