using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF1 RID: 3569
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).\n The scale is relative to Unity's Default Avatar")]
	public class GetAnimatorHumanScale : ComponentAction<Animator>
	{
		// Token: 0x0600670E RID: 26382 RVA: 0x00209223 File Offset: 0x00207423
		public override void Reset()
		{
			this.gameObject = null;
			this.humanScale = null;
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x00209233 File Offset: 0x00207433
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.humanScale.Value = this.cachedComponent.humanScale;
			}
			base.Finish();
		}

		// Token: 0x0400665E RID: 26206
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400665F RID: 26207
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("the scale of the current Avatar")]
		public FsmFloat humanScale;
	}
}
