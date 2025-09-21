using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E06 RID: 3590
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Set Apply Root Motion: If true, Root is controlled by animations")]
	public class SetAnimatorApplyRootMotion : ComponentAction<Animator>
	{
		// Token: 0x06006777 RID: 26487 RVA: 0x0020A22C File Offset: 0x0020842C
		public override void Reset()
		{
			this.gameObject = null;
			this.applyRootMotion = null;
		}

		// Token: 0x06006778 RID: 26488 RVA: 0x0020A23C File Offset: 0x0020843C
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.applyRootMotion = this.applyRootMotion.Value;
			}
			base.Finish();
		}

		// Token: 0x040066BA RID: 26298
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066BB RID: 26299
		[Tooltip("If true, Root motion is controlled by animations")]
		public FsmBool applyRootMotion;
	}
}
