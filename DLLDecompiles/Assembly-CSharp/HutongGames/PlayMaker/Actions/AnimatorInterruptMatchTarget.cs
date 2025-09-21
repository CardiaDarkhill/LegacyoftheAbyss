using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDA RID: 3546
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Interrupts the automatic target matching. CompleteMatch will make the GameObject match the target completely at the next frame.")]
	public class AnimatorInterruptMatchTarget : ComponentAction<Animator>
	{
		// Token: 0x0600669D RID: 26269 RVA: 0x00207F80 File Offset: 0x00206180
		public override void Reset()
		{
			this.gameObject = null;
			this.completeMatch = true;
		}

		// Token: 0x0600669E RID: 26270 RVA: 0x00207F95 File Offset: 0x00206195
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.InterruptMatchTarget(this.completeMatch.Value);
			}
			base.Finish();
		}

		// Token: 0x040065F4 RID: 26100
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065F5 RID: 26101
		[Tooltip("Will make the GameObject match the target completely at the next frame")]
		public FsmBool completeMatch;
	}
}
