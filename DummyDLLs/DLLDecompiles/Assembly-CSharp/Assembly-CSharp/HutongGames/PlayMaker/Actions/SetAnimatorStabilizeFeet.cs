using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E14 RID: 3604
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("If true, automatically stabilize feet during transition and blending")]
	public class SetAnimatorStabilizeFeet : ComponentAction<Animator>
	{
		// Token: 0x060067BD RID: 26557 RVA: 0x0020AF59 File Offset: 0x00209159
		public override void Reset()
		{
			this.gameObject = null;
			this.stabilizeFeet = null;
		}

		// Token: 0x060067BE RID: 26558 RVA: 0x0020AF69 File Offset: 0x00209169
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.stabilizeFeet = this.stabilizeFeet.Value;
			}
			base.Finish();
		}

		// Token: 0x040066FC RID: 26364
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066FD RID: 26365
		[Tooltip("If true, automatically stabilize feet during transition and blending")]
		public FsmBool stabilizeFeet;
	}
}
