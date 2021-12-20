using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimStateAttackEnd : StateMachineBehaviour
{
	public UnityEvent OnAnimationEnd;
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		OnAnimationEnd?.Invoke();
	}
}

