using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC3 RID: 3267
	[ActionCategory(ActionCategory.Physics2D)]
	public class MultiplyGravity : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600618B RID: 24971 RVA: 0x001EE732 File Offset: 0x001EC932
		public override void Reset()
		{
			this.gameObject = null;
			this.multiplyBy = 1f;
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x001EE74B File Offset: 0x001EC94B
		public override void OnEnter()
		{
			this.DoSetGravityScale();
			base.Finish();
		}

		// Token: 0x0600618D RID: 24973 RVA: 0x001EE75C File Offset: 0x001EC95C
		private void DoSetGravityScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.gravityScale *= this.multiplyBy.Value;
		}

		// Token: 0x04005FBC RID: 24508
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody 2d attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005FBD RID: 24509
		[RequiredField]
		[Tooltip("The gravity scale effect")]
		public FsmFloat multiplyBy;
	}
}
