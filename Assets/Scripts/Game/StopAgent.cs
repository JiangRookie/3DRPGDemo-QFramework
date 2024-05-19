using UnityEngine;
using UnityEngine.AI;

public class StopAgent : StateMachineBehaviour
{
	// 当转换开始并且状态机开始评估此状态时，会调用OnStateEnter
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<NavMeshAgent>().isStopped = true;
	}

	// OnStateUpdate在OnStateEnter和OnStateExit回调之间的每个Update帧上被调用
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<NavMeshAgent>().isStopped = true;
	}

	// 当转换结束并且状态机完成评估此状态时，会调用OnStateExit
	// public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	// {
	// 	animator.GetComponent<NavMeshAgent>().isStopped = false;
	// }

	// OnStateMove在Animator.OnAnimatorMove()之后立即被调用
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // 实现处理和影响根运动的代码
	//}

	// OnStateIK在Animator.OnAnimatorIK()之后立即被调用
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // 实现设置动画IK（反向运动学）的代码
	//}
}