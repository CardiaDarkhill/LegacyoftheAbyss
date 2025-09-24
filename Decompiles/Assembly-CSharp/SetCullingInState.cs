using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class SetCullingInState : StateMachineBehaviour
{
	// Token: 0x06000518 RID: 1304 RVA: 0x0001A6F7 File Offset: 0x000188F7
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.cullingMode = this.cullingMode;
	}

	// Token: 0x040004F4 RID: 1268
	[SerializeField]
	private AnimatorCullingMode cullingMode;
}
