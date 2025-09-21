using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9F RID: 2975
	[ActionCategory("Trail Renderer")]
	[Tooltip("Set trail renderer parameters")]
	public class ActivateTrailRenderer : FsmStateAction
	{
		// Token: 0x06005BF3 RID: 23539 RVA: 0x001CEB27 File Offset: 0x001CCD27
		public override void Reset()
		{
			this.gameObject = null;
			this.activate = null;
		}

		// Token: 0x06005BF4 RID: 23540 RVA: 0x001CEB37 File Offset: 0x001CCD37
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<TrailRenderer>().enabled = this.activate.Value;
				base.Finish();
				return;
			}
			base.Finish();
		}

		// Token: 0x04005755 RID: 22357
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005756 RID: 22358
		public FsmBool activate;
	}
}
