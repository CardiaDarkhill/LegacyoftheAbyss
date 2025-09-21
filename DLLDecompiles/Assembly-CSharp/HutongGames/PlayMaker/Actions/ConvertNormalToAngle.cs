using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB0 RID: 3504
	[ActionCategory("Physics 2d")]
	public class ConvertNormalToAngle : RigidBody2dActionBase
	{
		// Token: 0x060065B0 RID: 26032 RVA: 0x0020175F File Offset: 0x001FF95F
		public override void Reset()
		{
			this.contactNormal = null;
			this.storeAngle = null;
			this.everyFrame = false;
		}

		// Token: 0x060065B1 RID: 26033 RVA: 0x00201776 File Offset: 0x001FF976
		public override void OnEnter()
		{
			this.Convert();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x0020178C File Offset: 0x001FF98C
		public override void OnUpdate()
		{
			this.Convert();
		}

		// Token: 0x060065B3 RID: 26035 RVA: 0x00201794 File Offset: 0x001FF994
		private void Convert()
		{
			Vector2 vector = new Vector2(this.contactNormal.Value.x, this.contactNormal.Value.y);
			float num = Mathf.Atan2(vector.x, -vector.y) * 180f / 3.1415927f - 90f;
			if (num < 0f)
			{
				num += 360f;
			}
			this.storeAngle.Value = num;
		}

		// Token: 0x040064D7 RID: 25815
		public FsmVector3 contactNormal;

		// Token: 0x040064D8 RID: 25816
		public FsmFloat storeAngle;

		// Token: 0x040064D9 RID: 25817
		public bool everyFrame;
	}
}
