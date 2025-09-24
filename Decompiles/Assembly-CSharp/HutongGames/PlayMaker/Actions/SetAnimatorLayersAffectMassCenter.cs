using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E0E RID: 3598
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("If true, additional layers affects the mass center")]
	public class SetAnimatorLayersAffectMassCenter : ComponentAction<Animator>
	{
		// Token: 0x0600679F RID: 26527 RVA: 0x0020AA6C File Offset: 0x00208C6C
		public override void Reset()
		{
			this.gameObject = null;
			this.affectMassCenter = null;
		}

		// Token: 0x060067A0 RID: 26528 RVA: 0x0020AA7C File Offset: 0x00208C7C
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.layersAffectMassCenter = this.affectMassCenter.Value;
			}
			base.Finish();
		}

		// Token: 0x040066E2 RID: 26338
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066E3 RID: 26339
		[Tooltip("If true, additional layers affects the mass center")]
		public FsmBool affectMassCenter;
	}
}
