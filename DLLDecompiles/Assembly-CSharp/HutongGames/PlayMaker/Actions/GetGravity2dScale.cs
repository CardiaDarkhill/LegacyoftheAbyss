using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC0 RID: 4032
	[ActionCategory(ActionCategory.Physics2D)]
	public class GetGravity2dScale : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F48 RID: 28488 RVA: 0x0022646B File Offset: 0x0022466B
		public override void Reset()
		{
			this.gameObject = null;
			this.storeGravity = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F49 RID: 28489 RVA: 0x00226482 File Offset: 0x00224682
		public override void OnEnter()
		{
			this.DoGetGravityScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F4A RID: 28490 RVA: 0x00226498 File Offset: 0x00224698
		public override void OnUpdate()
		{
			this.DoGetGravityScale();
		}

		// Token: 0x06006F4B RID: 28491 RVA: 0x002264A0 File Offset: 0x002246A0
		private void DoGetGravityScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			this.storeGravity.Value = base.rigidbody2d.gravityScale;
		}

		// Token: 0x04006EE0 RID: 28384
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody 2d attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EE1 RID: 28385
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeGravity;

		// Token: 0x04006EE2 RID: 28386
		public bool everyFrame;
	}
}
