using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E16 RID: 3606
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets a trigger parameter to active. Triggers are parameters that act mostly like booleans, but get reset to inactive when they are used in a transition.")]
	public class SetAnimatorTrigger : ComponentAction<Animator>
	{
		// Token: 0x060067C6 RID: 26566 RVA: 0x0020B031 File Offset: 0x00209231
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = null;
		}

		// Token: 0x060067C7 RID: 26567 RVA: 0x0020B041 File Offset: 0x00209241
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.SetTrigger(this.trigger.Value);
			}
			base.Finish();
		}

		// Token: 0x04006702 RID: 26370
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006703 RID: 26371
		[RequiredField]
		[UIHint(UIHint.AnimatorTrigger)]
		[Tooltip("The trigger name")]
		public FsmString trigger;
	}
}
