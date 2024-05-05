using QFramework;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public enum RockState { HitPlayer, HitEnemy, HitNothing, }

	public partial class Rock : ViewController
	{
		[SerializeField] private float _Force;
		[SerializeField] private GameObject _Target;
		[SerializeField] private int _Damage;
		private Vector3 _Direction;
		public RockState RockState { get; private set; }

		private void Start()
		{
			SelfRigidbody.velocity = Vector3.one;
			RockState = RockState.HitPlayer;
			FlyToTarget();
		}

		private void FixedUpdate()
		{
			if (SelfRigidbody.velocity.sqrMagnitude < 1)
			{
				RockState = RockState.HitNothing;
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			switch (RockState)
			{
				case RockState.HitPlayer:
					if (other.gameObject.CompareTag("Player"))
					{
						other.gameObject
						   .GetComponent<IPushable>()
						   .SetPushed(_Direction * _Force);
						PlayerData.TakeHurt(_Damage);
						RockState = RockState.HitNothing;
					}
					break;
				case RockState.HitEnemy:
					var golem = other.gameObject.GetComponent<Golem>();
					if (golem)
					{
						PlayerData.TakeDamage(_Damage, golem.SelfCharacterData);

						RockBreakEffect
						   .Instantiate()
						   .Position(this.Position())
						   .RotationIdentity()
						   .DestroySelf();
					}
					break;
				case RockState.HitNothing: break;
			}
		}

		public void HandleHit(Vector3 direction)
		{
			RockState = RockState.HitEnemy;
			SelfRigidbody.velocity = Vector3.one;
			SelfRigidbody.AddForce(direction * 20, ForceMode.Impulse);
		}

		private void FlyToTarget()
		{
			if (!_Target)
			{
				_Target = FindObjectOfType<Player>().gameObject; // TODO: 后续修改获取 Player 的方法
			}
			_Direction = (_Target.Position() - this.Position() + Vector3.up).normalized;
			SelfRigidbody.AddForce(_Direction * _Force, ForceMode.Impulse);
		}

		public void SetAttackTarget(GameObject attackTarget)
		{
			_Target = attackTarget;
		}
	}
}