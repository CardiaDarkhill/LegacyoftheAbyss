using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD5 RID: 3029
	[ActionCategory("Physics 2d")]
	[Tooltip("Get the dimensions of the BoxCollider 2D")]
	public class BoundsBoxCollider : FsmStateAction
	{
		// Token: 0x06005CE5 RID: 23781 RVA: 0x001D3225 File Offset: 0x001D1425
		public override void Reset()
		{
			this.gameObject1 = null;
			this.scaleX = null;
			this.scaleY = null;
			this.everyFrame = false;
		}

		// Token: 0x06005CE6 RID: 23782 RVA: 0x001D3244 File Offset: 0x001D1444
		public void GetEm()
		{
			Vector2 vector = base.Fsm.GetOwnerDefaultTarget(this.gameObject1).GetComponent<BoxCollider2D>().bounds.size;
			if (this.scaleVector2 != null)
			{
				this.scaleVector2.Value = vector;
			}
			if (this.scaleX != null)
			{
				this.scaleX.Value = vector.x;
			}
			if (this.scaleY != null)
			{
				this.scaleY.Value = vector.y;
			}
		}

		// Token: 0x06005CE7 RID: 23783 RVA: 0x001D32C0 File Offset: 0x001D14C0
		public override void OnEnter()
		{
			this.GetEm();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CE8 RID: 23784 RVA: 0x001D32D6 File Offset: 0x001D14D6
		public override void OnUpdate()
		{
			this.GetEm();
		}

		// Token: 0x0400587E RID: 22654
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		// Token: 0x0400587F RID: 22655
		[Tooltip("Store the dimensions of the BoxCollider2D")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 scaleVector2;

		// Token: 0x04005880 RID: 22656
		[UIHint(UIHint.Variable)]
		public FsmFloat scaleX;

		// Token: 0x04005881 RID: 22657
		[UIHint(UIHint.Variable)]
		public FsmFloat scaleY;

		// Token: 0x04005882 RID: 22658
		public bool everyFrame;
	}
}
