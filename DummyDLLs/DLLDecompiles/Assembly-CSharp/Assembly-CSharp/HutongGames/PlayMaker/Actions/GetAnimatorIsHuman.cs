using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF4 RID: 3572
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if the current rig is humanoid, false if it is generic. Can also sends events")]
	public class GetAnimatorIsHuman : ComponentAction<Animator>
	{
		// Token: 0x0600671D RID: 26397 RVA: 0x00209520 File Offset: 0x00207720
		public override void Reset()
		{
			this.gameObject = null;
			this.isHuman = null;
			this.isHumanEvent = null;
			this.isGenericEvent = null;
		}

		// Token: 0x0600671E RID: 26398 RVA: 0x00209540 File Offset: 0x00207740
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				bool flag = this.cachedComponent.isHuman;
				if (!this.isHuman.IsNone)
				{
					this.isHuman.Value = flag;
				}
				base.Fsm.Event(flag ? this.isHumanEvent : this.isGenericEvent);
			}
			base.Finish();
		}

		// Token: 0x0400666F RID: 26223
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006670 RID: 26224
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the current rig is humanoid, False if it is generic")]
		public FsmBool isHuman;

		// Token: 0x04006671 RID: 26225
		[Tooltip("Event send if rig is humanoid")]
		public FsmEvent isHumanEvent;

		// Token: 0x04006672 RID: 26226
		[Tooltip("Event send if rig is generic")]
		public FsmEvent isGenericEvent;
	}
}
