// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
using QFramework;
using UnityEngine;

namespace Game
{
	public partial class Golem : BaseMonster
	{
		[SerializeField] private float _PushingForce = 30f;

		// Animation Event
		public void Push()
		{
			if (!_AttackTarget) return;
			if (!transform.IsFacingTarget(_AttackTarget.transform)) return;
			var pushedToPosition = _AttackTarget.NormalizedDirectionFrom(gameObject) * _PushingForce;
			_AttackTarget
			   .GetComponent<IPushable>()
			   .SetPushed(pushedToPosition);
			TakeDamage(SelfCharacterData, () => _AttackTarget.GetComponent<IGetHit>().GetHit());
		}

		// Animation Event
		public void ThrowRock()
		{
			if (_AttackTarget)
			{
				RockGenerate.GenerateRock(RightHandPos.Position(), _AttackTarget);
			}
		}
	}
}