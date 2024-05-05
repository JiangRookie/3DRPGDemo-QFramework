// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
using QFramework;
using UnityEngine;

namespace Game
{
	public partial class Golem : BaseMonster
	{
		public static EasyEvent<GameObject> OnRockThrow = new EasyEvent<GameObject>();
		[SerializeField] private float _PushingForce = 30f;

		public void Push()
		{
			if (_AttackTarget && transform.IsFacingTarget(_AttackTarget.transform))
			{
				_AttackTarget.GetComponent<IPushable>()
				   .SetPushed(_AttackTarget.NormalizedDirectionFrom(gameObject) * _PushingForce);
				TakeDamage(SelfCharacterData, () => _AttackTarget.GetComponent<IGetHit>().GetHit());
			}
		}

		// Animation Event
		public void ThrowRock()
		{
			if (_AttackTarget)
			{
				Rock.Instantiate()
				   .Position(RightHandPos.Position())
				   .RotationIdentity();
				OnRockThrow.Trigger(_AttackTarget);
			}
		}
	}
}