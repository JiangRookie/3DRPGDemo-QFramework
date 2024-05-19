using System.Collections.Generic;
using Game.SO;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public partial class Player : ViewController, IGetHit, IPushable
	{
		public List<AttackCombo_SO> ComboList;
		private float _AttackCooldown;
		private GameObject _AttackTarget;
		private int _ComboCounter;
		private bool _IsAttacking = false;
		private bool _IsDead;
		private float _LastClickedTime;
		private float _LastCombaEnd;
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
			ExitAttack();
			if (_IsAttacking)
			{
				if (_AttackTarget && _AttackTarget.Distance(this) > PlayerData.AttackRange.Value)
				{
					SelfNavMeshAgent.destination = _AttackTarget.Position();
				}
				else
				{
					SelfNavMeshAgent.isStopped = true;
					AttackTarget();
				}
			}
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
				SelfNavMeshAgent.isStopped = false;
				SelfNavMeshAgent.stoppingDistance = PlayerData.AttackRange.Value;
				transform.LookAt(_AttackTarget.transform);
				_IsAttacking = true;
			}
		}

		private void AttackTarget()
		{
			if (_AttackCooldown < 0)
			{
				PlayerData.IsCritical.Value = Random.value <= PlayerData.CriticalHitRate.Value;
				SelfAnimator.SetBool(AnimatorHash.Critical, PlayerData.IsCritical.Value);

				// SelfAnimator.SetTrigger(AnimatorHash.Attack);
				Attack();
				_AttackCooldown = PlayerData.CoolDown.Value;
			}
			else
			{
				_IsAttacking = false;
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
					PlayerData.InflictDamage(_AttackTarget.GetComponent<CharacterData>(),
						() =>
						{
							_AttackTarget.GetComponent<IGetHit>().GetHit();
						});
				}
			}
		}

		private void Attack()
		{
			if (Time.time - _LastCombaEnd > 0.2f)
			{
				CancelInvoke(nameof(EndCombo));
				if (Time.time - _LastClickedTime >= 0.2f)
				{
					if (_ComboCounter >= ComboList.Count)
					{
						_ComboCounter = 0;
					}
					SelfAnimator.runtimeAnimatorController = ComboList[_ComboCounter].AnimatorOV;
					SelfAnimator.Play("Attack", 0, 0);
					PlayerData.MinDamage.Value = ComboList[_ComboCounter].Damage;
					PlayerData.MaxDamage.Value = ComboList[_ComboCounter].Damage;
					_ComboCounter++;
					_LastClickedTime = Time.time;
				}
			}
		}

		private void ExitAttack()
		{
			if (SelfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && SelfAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
			{
				Invoke(nameof(EndCombo), 1f);
			}
		}

		private void EndCombo()
		{
			_ComboCounter = 0;
			_LastCombaEnd = Time.time;
		}
	}
}