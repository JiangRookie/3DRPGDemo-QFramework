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

			Golem.OnRockThrow
			   .Register(target => _Target = target)
			   .UnRegisterWhenGameObjectDestroyed(gameObject);

			Player.OnHitRock
			   .Register(direction =>
				{
					RockState = RockState.HitEnemy;
					SelfRigidbody.velocity = Vector3.one;
					SelfRigidbody.AddForce(direction * 20, ForceMode.Impulse);
				})
			   .UnRegisterWhenGameObjectDestroyed(gameObject);
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
						other.gameObject.GetComponent<IPushable>().SetPushed(_Direction * _Force);
						int damage = Mathf.Max(_Damage - PlayerData.CurDefense.Value, 0);
						PlayerData.CurHealth.Value = Mathf.Max(PlayerData.CurHealth.Value - damage, 0);
						RockState = RockState.HitNothing;
					}
					break;
				case RockState.HitEnemy:

					var golem = other.gameObject.GetComponent<Golem>();
					if (golem)
					{
						int damage = Mathf.Max(_Damage - golem.SelfCharacterData.CurDefense, 0);
						golem.SelfCharacterData.CurHealth = Mathf.Max(golem.SelfCharacterData.CurHealth - damage, 0);
						RockBreakEffect.Instantiate()
						   .Position(this.Position())
						   .RotationIdentity();
						Destroy(gameObject);
					}
					break;
				case RockState.HitNothing: break;
			}
		}

		private void FlyToTarget()
		{
			if (!_Target)
			{
				_Target = FindObjectOfType<Player>().gameObject;
			}
			_Direction = (_Target.Position() - this.Position() + Vector3.up).normalized;
			SelfRigidbody.AddForce(_Direction * _Force, ForceMode.Impulse);
		}
	}
}