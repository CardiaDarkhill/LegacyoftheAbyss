using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FDA RID: 4058
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets The degree to which this object is affected by gravity.  NOTE: Game object must have a rigidbody 2D.")]
	public class SetGravity2dScale : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FC9 RID: 28617 RVA: 0x00228FE5 File Offset: 0x002271E5
		public override void Reset()
		{
			this.gameObject = null;
			this.gravityScale = 1f;
		}

		// Token: 0x06006FCA RID: 28618 RVA: 0x00228FFE File Offset: 0x002271FE
		public override void OnEnter()
		{
			this.DoSetGravityScale();
			base.Finish();
		}

		// Token: 0x06006FCB RID: 28619 RVA: 0x0022900C File Offset: 0x0022720C
		private void DoSetGravityScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.gravityScale = this.gravityScale.Value;
		}

		// Token: 0x04006FC2 RID: 28610
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody 2d attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FC3 RID: 28611
		[RequiredField]
		[Tooltip("The gravity scale effect")]
		public FsmFloat gravityScale;
	}
}
