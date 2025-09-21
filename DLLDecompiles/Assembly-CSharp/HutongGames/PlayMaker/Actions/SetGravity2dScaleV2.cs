using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2D RID: 3373
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets The degree to which this object is affected by gravity.  NOTE: Game object must have a rigidbody 2D.")]
	public class SetGravity2dScaleV2 : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600634E RID: 25422 RVA: 0x001F5E96 File Offset: 0x001F4096
		public override void Reset()
		{
			this.gameObject = null;
			this.gravityScale = 1f;
			this.everyFrame = false;
		}

		// Token: 0x0600634F RID: 25423 RVA: 0x001F5EB6 File Offset: 0x001F40B6
		public override void OnEnter()
		{
			this.DoSetGravityScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006350 RID: 25424 RVA: 0x001F5ECC File Offset: 0x001F40CC
		public override void OnUpdate()
		{
			this.DoSetGravityScale();
		}

		// Token: 0x06006351 RID: 25425 RVA: 0x001F5ED4 File Offset: 0x001F40D4
		private void DoSetGravityScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.gravityScale = this.gravityScale.Value;
		}

		// Token: 0x040061B5 RID: 25013
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody 2d attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061B6 RID: 25014
		[RequiredField]
		[Tooltip("The gravity scale effect")]
		public FsmFloat gravityScale;

		// Token: 0x040061B7 RID: 25015
		public bool everyFrame;
	}
}
