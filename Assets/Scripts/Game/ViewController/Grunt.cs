// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
using QFramework;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
	public partial class Grunt : BaseMonster
	{
		private static readonly int s_Dizzy = Animator.StringToHash("Dizzy");
		[SerializeField] private float _PushingForce = 10f;

		public void Push()
		{
			if (_AttackTarget)
			{
				transform.LookAt(_AttackTarget.transform);
				var direction = (_AttackTarget.Position() - this.Position()).normalized;
				_AttackTarget.GetComponent<NavMeshAgent>().isStopped = true;
				_AttackTarget.GetComponent<NavMeshAgent>().velocity = direction * _PushingForce;
				_AttackTarget.GetComponent<Animator>().SetTrigger(s_Dizzy);
			}
		}
	}
}