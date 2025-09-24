using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD6 RID: 3030
	[ActionCategory("Physics 2d")]
	[Tooltip("Get the offset of the BoxCollider 2D as a Vector2")]
	public class BoxColliderOffset : FsmStateAction
	{
		// Token: 0x06005CEA RID: 23786 RVA: 0x001D32E6 File Offset: 0x001D14E6
		public override void Reset()
		{
			this.gameObject1 = null;
			this.offsetX = null;
			this.offsetY = null;
			this.everyFrame = false;
		}

		// Token: 0x06005CEB RID: 23787 RVA: 0x001D3304 File Offset: 0x001D1504
		public void GetOffset()
		{
			Vector2 offset = base.Fsm.GetOwnerDefaultTarget(this.gameObject1).GetComponent<BoxCollider2D>().offset;
			if (this.offsetVector2 != null)
			{
				this.offsetVector2.Value = offset;
			}
			if (this.offsetX != null)
			{
				this.offsetX.Value = offset.x;
			}
			if (this.offsetY != null)
			{
				this.offsetY.Value = offset.y;
			}
		}

		// Token: 0x06005CEC RID: 23788 RVA: 0x001D3373 File Offset: 0x001D1573
		public override void OnEnter()
		{
			this.GetOffset();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CED RID: 23789 RVA: 0x001D3389 File Offset: 0x001D1589
		public override void OnUpdate()
		{
			this.GetOffset();
		}

		// Token: 0x04005883 RID: 22659
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		// Token: 0x04005884 RID: 22660
		[Tooltip("Vector2 where the offset of the BoxCollider2D is stored")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 offsetVector2;

		// Token: 0x04005885 RID: 22661
		[UIHint(UIHint.Variable)]
		public FsmFloat offsetX;

		// Token: 0x04005886 RID: 22662
		[UIHint(UIHint.Variable)]
		public FsmFloat offsetY;

		// Token: 0x04005887 RID: 22663
		public bool everyFrame;
	}
}
