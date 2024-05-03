using QFramework;
using UnityEngine;
using UnityEngine.AI;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public enum EnemyState { GUARD, PATROL, CHASE, DEAD }

	[RequireComponent(typeof(NavMeshAgent))]
	public partial class Slime : ViewController
	{
		[SerializeField] private EnemyState _EnemyState;

		private void Update()
		{
			SwitchStates();
		}

		private void SwitchStates()
		{
			switch (_EnemyState)
			{
				case EnemyState.GUARD: break;
				case EnemyState.PATROL: break;
				case EnemyState.CHASE: break;
				case EnemyState.DEAD: break;
			}
		}
	}
}