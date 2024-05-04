using System.Collections;
using QFramework;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public partial class Player : ViewController
	{
		private static readonly int s_Speed = Animator.StringToHash("Speed");
		private static readonly int s_Attack = Animator.StringToHash("Attack");
		private float _AttackCooldown;
		private GameObject _AttackTarget;
		private static readonly int s_Critical = Animator.StringToHash("Critical");

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
			SwitchAnimation();

			_AttackCooldown -= Time.deltaTime;
		}

		private void SwitchAnimation()
		{
			SelfAnimator.SetFloat(s_Speed, SelfNavMeshAgent.velocity.sqrMagnitude);
		}

		private void MoveToTarget(Vector3 targetPoint)
		{
			StopAllCoroutines();
			SelfNavMeshAgent.isStopped = false;
			SelfNavMeshAgent.destination = targetPoint;
		}

		private void MoveToAttackTarget(GameObject targetGameObj)
		{
			if (targetGameObj)
			{
				_AttackTarget = targetGameObj;
				SelfCharacterData.IsCritical = Random.value < SelfCharacterData.CriticalHitRate;
				StartCoroutine(MoveToAttackTargetCoroutine());
			}
		}

		private IEnumerator MoveToAttackTargetCoroutine()
		{
			SelfNavMeshAgent.isStopped = false;

			transform.LookAt(_AttackTarget.transform);

			while (Vector3.Distance(_AttackTarget.Position(), this.Position()) > SelfCharacterData.AttackRange)
			{
				SelfNavMeshAgent.destination = _AttackTarget.Position();
				yield return null;
			}

			SelfNavMeshAgent.isStopped = true;

			if (_AttackCooldown < 0)
			{
				SelfAnimator.SetBool(s_Critical, SelfCharacterData.IsCritical);
				SelfAnimator.SetTrigger(s_Attack);
				_AttackCooldown = SelfCharacterData.CoolDown;
			}
		}

		public void Hit()
		{
			if (_AttackTarget)
			{
				DamageCalculator.TakeDamage(SelfCharacterData, _AttackTarget.GetComponent<CharacterData>());
			}
		}
	}
}