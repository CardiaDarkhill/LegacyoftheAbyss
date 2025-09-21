using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFD RID: 3325
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets a trigger parameter to active. Triggers are parameters that act mostly like booleans, but get reset to inactive when they are used in a transition.")]
	public class ResetAnimatorTrigger : ComponentAction<Animator>
	{
		// Token: 0x06006289 RID: 25225 RVA: 0x001F2ADA File Offset: 0x001F0CDA
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = null;
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x001F2AEA File Offset: 0x001F0CEA
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.ResetTrigger(this.trigger.Value);
			}
			base.Finish();
		}

		// Token: 0x040060F5 RID: 24821
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040060F6 RID: 24822
		[RequiredField]
		[UIHint(UIHint.AnimatorTrigger)]
		[Tooltip("The trigger name")]
		public FsmString trigger;
	}
}
