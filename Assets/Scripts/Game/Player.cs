using QFramework;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public partial class Player : ViewController
	{
		private static readonly int s_Speed = Animator.StringToHash("Speed");

		private void Start()
		{
			MouseManager.OnMouseClicked
			   .Register(MoveToTarget)
			   .UnRegisterWhenDisabled(gameObject);
		}

		private void Update()
		{
			SwitchAnimation();
		}

		private void SwitchAnimation()
		{
			SelfAnimator.SetFloat(s_Speed, SelfNavMeshAgent.velocity.sqrMagnitude);
		}

		private void MoveToTarget(Vector3 targetPoint)
		{
			SelfNavMeshAgent.destination = targetPoint;
		}
	}
}