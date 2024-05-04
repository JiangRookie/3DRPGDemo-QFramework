using System;
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
		private static readonly int s_Speed = Animator.StringToHash("Speed");
		private static readonly int s_Attack = Animator.StringToHash("Attack");
		private static readonly int s_Critical = Animator.StringToHash("Critical");
		private static readonly int s_Die = Animator.StringToHash("Die");
		private float _AttackCooldown;
		private GameObject _AttackTarget;
		private bool _IsDead;
		private static readonly int s_GetHit = Animator.StringToHash("GetHit");
		private static readonly int s_Dizzy = Animator.StringToHash("Dizzy");

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

		private void SwitchAnimation()
		{
			SelfAnimator.SetFloat(s_Speed, SelfNavMeshAgent.velocity.sqrMagnitude);
			SelfAnimator.SetBool(s_Die, _IsDead);
		}

		private void MoveToTarget(Vector3 targetPoint)
		{
			StopAllCoroutines();
			if (_IsDead) return;
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

			transform.LookAt(_AttackTarget.transform);

			while (Vector3.Distance(_AttackTarget.Position(), this.Position()) > PlayerData.AttackRange.Value + +SelfNavMeshAgent.stoppingDistance)
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
				SelfAnimator.SetBool(s_Critical, PlayerData.IsCritical.Value);
				SelfAnimator.SetTrigger(s_Attack);
				_AttackCooldown = PlayerData.CoolDown.Value;
			}
		}

		public void Hit()
		{
			if (_AttackTarget)
			{
				TakeDamage(_AttackTarget.GetComponent<CharacterData>(), () =>
				{
					_AttackTarget.GetComponent<IGetHit>().GetHit();
				});
			}
		}

		public void GetHit()
		{
			SelfAnimator.SetTrigger(s_GetHit);
		}

		private void TakeDamage(CharacterData target, Action criticalAction)
		{
			float baseDamage = Random.Range(PlayerData.MinDamage.Value, PlayerData.MaxDamage.Value + 1);
			if (PlayerData.IsCritical.Value)
			{
				baseDamage *= PlayerData.CriticalHitBonusPercentage.Value;
				criticalAction?.Invoke();
			}
			int realDamage = Mathf.Max((int)baseDamage - target.CurDefense, 1);
			target.CurHealth = Mathf.Max(target.CurHealth - realDamage, 0);
		}

		public void GetPushed(Vector3 pushedToPosition)
		{
			SelfNavMeshAgent.isStopped = true;
			SelfNavMeshAgent.velocity = pushedToPosition;
			SelfAnimator.SetTrigger(s_Dizzy);
		}
	}
}