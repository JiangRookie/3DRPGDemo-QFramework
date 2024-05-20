using QFramework;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public partial class Rock : ViewController
	{
		[SerializeField] private float _Force;
		[SerializeField] private int _Damage;
		private enum RockState { HitPlayer, HitEnemy, HitNothing }
		private RockState _RockState = RockState.HitPlayer;
		private GameObject _AttackTarget;
		private Vector3 _Direction;

		private void Start()
		{
			// 设置Rock对象的初始速度为Vector3.one，这样可以确保在FixedUpdate方法中，
			// Rock对象的速度的平方模长不会小于1，避免Rock对象的状态在游戏开始时就被设置为RockState.HitNothing
			SelfRigidbody.velocity = Vector3.one;
			FlyToTarget();
		}

		private void FixedUpdate()
		{
			if (SelfRigidbody.velocity.sqrMagnitude < 1)
			{
				_RockState = RockState.HitNothing;
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			switch (_RockState)
			{
				case RockState.HitPlayer:
					if (other.gameObject.CompareTag("Player"))
					{
						other.gameObject
						   .GetComponent<IPushable>()
						   .SetPushed(_Direction * _Force);
						PlayerData.TakeHurt(_Damage);
						_RockState = RockState.HitNothing;
					}
					break;
				case RockState.HitEnemy:
					var golem = other.gameObject.GetComponent<Golem>();
					if (golem)
					{
						PlayerData.InflictDamage(golem.SelfCharacterData, _Damage);

						RockBreakEffect
						   .Instantiate()
						   .Position(this.Position())
						   .RotationIdentity();

						this.DestroyGameObj();
					}
					break;
			}
		}

		public void SetAttackTarget(GameObject attackTarget)
		{
			_AttackTarget = attackTarget;
		}

		public void TryHandleHit(Vector3 direction)
		{
			if (_RockState != RockState.HitNothing) return;
			_RockState = RockState.HitEnemy;
			SelfRigidbody.velocity = Vector3.one;
			SelfRigidbody.AddForce(direction * 20, ForceMode.Impulse);
		}

		private void FlyToTarget()
		{
			if (!_AttackTarget)
			{
				_AttackTarget = FindObjectOfType<Player>().gameObject;
			}
			_Direction = (_AttackTarget.Position() - this.Position() + Vector3.up).normalized;
			SelfRigidbody.AddForce(_Direction * _Force, ForceMode.Impulse);
		}
	}
}