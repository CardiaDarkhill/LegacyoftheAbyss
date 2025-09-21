using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C62 RID: 3170
	[ActionCategory("Physics 2d")]
	[Tooltip("Returns the X/Y Min and max bounds for a box2d collider (in world space)")]
	public class GetColliderRange : FsmStateAction
	{
		// Token: 0x06005FDB RID: 24539 RVA: 0x001E60BC File Offset: 0x001E42BC
		public override void Reset()
		{
			this.gameObject = null;
			this.minX = null;
			this.maxX = null;
			this.minY = null;
			this.maxY = null;
			this.everyFrame = false;
		}

		// Token: 0x06005FDC RID: 24540 RVA: 0x001E60E8 File Offset: 0x001E42E8
		public override void OnEnter()
		{
			this.box = this.gameObject.GetSafe(this);
			bool flag = this.box != null;
			if (flag)
			{
				this.GetRange();
			}
			if (!flag || !this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FDD RID: 24541 RVA: 0x001E6124 File Offset: 0x001E4324
		public void GetRange()
		{
			Bounds bounds = this.box.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			this.minY.Value = min.y;
			this.maxY.Value = max.y;
			this.minX.Value = min.x;
			this.maxX.Value = max.x;
		}

		// Token: 0x06005FDE RID: 24542 RVA: 0x001E6191 File Offset: 0x001E4391
		public override void OnUpdate()
		{
			this.GetRange();
		}

		// Token: 0x04005D32 RID: 23858
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D33 RID: 23859
		[UIHint(UIHint.Variable)]
		public FsmFloat minX;

		// Token: 0x04005D34 RID: 23860
		[UIHint(UIHint.Variable)]
		public FsmFloat maxX;

		// Token: 0x04005D35 RID: 23861
		[UIHint(UIHint.Variable)]
		public FsmFloat minY;

		// Token: 0x04005D36 RID: 23862
		[UIHint(UIHint.Variable)]
		public FsmFloat maxY;

		// Token: 0x04005D37 RID: 23863
		public bool everyFrame;

		// Token: 0x04005D38 RID: 23864
		private BoxCollider2D box;
	}
}
