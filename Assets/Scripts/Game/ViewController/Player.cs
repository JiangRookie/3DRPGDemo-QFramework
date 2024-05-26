using System.Collections.Generic;
using DG.Tweening;
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
		private float _InitStopDistance;
		private bool _IsAttacking = false;
		private bool _IsDead;
		private float _LastClickedTime;
		private float _LastCombaEnd;

		private void Awake()
		{
			_InitStopDistance = SelfNavMeshAgent.stoppingDistance;
		}

		private void Start()
		{
			MouseManager.OnMouseClicked
			   .Register(MoveToTarget)
			   .UnRegisterWhenGameObjectDestroyed(gameObject);

			MouseManager.OnEnemyClicked
			   .Register(MoveToAttackTarget)
			   .UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurHealth.RegisterWithInitValue(value =>
			{
				if (value > 0) return;
				_IsDead = true;
				GameManager.Instance.GameEndNotify();
			}).UnRegisterWhenCurrentSceneUnloaded();
		}

		private void Update()
		{
			_AttackCooldown -= Time.deltaTime;
			SwitchAnimation();
			ExitAttack();
			if (_IsAttacking)
			{
				if (_AttackTarget && _AttackTarget.Distance(this) > PlayerData.AttackRange.Value + _InitStopDistance)
				{
					SelfNavMeshAgent.destination = _AttackTarget.Position();
				}
				else
				{
					SelfNavMeshAgent.isStopped = true;
					AttackTarget();
				}
			}

			if (Time.time - _LastClickedTime > 1.2f)
			{
				_ComboCounter = 0;
			}
		}

		void IGetHit.GetHit()
		{
			SelfAnimator.SetTrigger(AnimatorHash.GetHit);
			_ComboCounter = 0;
		}

		void IPushable.SetPushed(Vector3 pushedToPosition)
		{
			SelfNavMeshAgent.isStopped = true;
			SelfNavMeshAgent.velocity = pushedToPosition;
			SelfAnimator.SetTrigger(AnimatorHash.Dizzy);
			_ComboCounter = 0;
		}

		private void SwitchAnimation()
		{
			SelfAnimator.SetFloat(AnimatorHash.Speed, SelfNavMeshAgent.velocity.sqrMagnitude);
			SelfAnimator.SetBool(AnimatorHash.Die, _IsDead);
		}

		private void MoveToTarget(Vector3 targetPoint)
		{
			if (_IsDead) return;
			_IsAttacking = false;
			SelfNavMeshAgent.stoppingDistance = _InitStopDistance;
			SelfNavMeshAgent.destination = targetPoint;
			SelfNavMeshAgent.isStopped = false;
		}

		private void MoveToAttackTarget(GameObject targetGameObj)
		{
			if (_IsDead) return;
			if (!targetGameObj) return;
			_IsAttacking = true;
			_AttackTarget = targetGameObj;
			SelfNavMeshAgent.isStopped = false;
			SelfNavMeshAgent.stoppingDistance = PlayerData.AttackRange.Value + _InitStopDistance;
			transform.DOLookAt(_AttackTarget.Position(), 0.25f);
		}

		private void AttackTarget()
		{
			if (_AttackCooldown < 0)
			{
				PlayerData.IsCritical.Value = Random.value <= PlayerData.CriticalHitRate.Value;
				SelfAnimator.SetBool(AnimatorHash.Critical, PlayerData.IsCritical.Value);
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
						SelfAnimator.Play(AnimatorHash.Attack, 0, 0);
						PlayerData.MinDamage.Value = ComboList[_ComboCounter].MinDamage;
						PlayerData.MaxDamage.Value = ComboList[_ComboCounter].MaxDamage;
						PlayerData.AttackRange.Value = ComboList[_ComboCounter].AttackRange;
						_ComboCounter++;
						_LastClickedTime = Time.time;
					}
				}
				_AttackCooldown = PlayerData.CoolDown.Value;
			}
			else
			{
				_IsAttacking = false;
			}
		}

		private void ExitAttack()
		{
			if (SelfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
				SelfAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
			{
				Invoke(nameof(EndCombo), 1f);
			}
		}

		private void EndCombo()
		{
			_ComboCounter = 0;
			_LastCombaEnd = Time.time;
		}

		// Call by Animation Event, deal damage to target
		public void Hit()
		{
			if (!_AttackTarget) return;
			if (_AttackTarget.CompareTag("Attackable"))
			{
				if (_AttackTarget.TryGetComponent(out Rock rock))
				{
					rock.TryHandleHit(transform.forward);
				}
			}
			else if (_AttackTarget.CompareTag("Enemy"))
			{
				int damage = PlayerData.DealDamageToEnemy(_AttackTarget.GetComponent<CharacterData>(),
					() =>
					{
						_AttackTarget.GetComponent<IGetHit>().GetHit();
					});
				DamageTextController.ShowDamage(_AttackTarget.Position() + Vector3.up, damage);
			}
		}
	}
}