using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C57 RID: 3159
	[ActionCategory("Math")]
	[Tooltip("Get the angle between two vector3 positions. 0 is right, 90 is up etc.")]
	public class GetAngleBetweenPoints : FsmStateAction
	{
		// Token: 0x06005FA4 RID: 24484 RVA: 0x001E5698 File Offset: 0x001E3898
		public override void Reset()
		{
		}

		// Token: 0x06005FA5 RID: 24485 RVA: 0x001E569A File Offset: 0x001E389A
		public override void OnEnter()
		{
			this.DoGetAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FA6 RID: 24486 RVA: 0x001E56B0 File Offset: 0x001E38B0
		public override void OnUpdate()
		{
			this.DoGetAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FA7 RID: 24487 RVA: 0x001E56C8 File Offset: 0x001E38C8
		private void DoGetAngle()
		{
			float num = this.point1.Value.y - this.point2.Value.y;
			float num2 = this.point1.Value.x - this.point2.Value.x;
			float num3;
			for (num3 = Mathf.Atan2(num, num2) * 57.295776f; num3 < 0f; num3 += 360f)
			{
			}
			this.storeAngle.Value = num3;
		}

		// Token: 0x04005CF8 RID: 23800
		[RequiredField]
		public FsmVector3 point1;

		// Token: 0x04005CF9 RID: 23801
		[RequiredField]
		public FsmVector3 point2;

		// Token: 0x04005CFA RID: 23802
		[RequiredField]
		public FsmFloat storeAngle;

		// Token: 0x04005CFB RID: 23803
		private FsmFloat x;

		// Token: 0x04005CFC RID: 23804
		private FsmFloat y;

		// Token: 0x04005CFD RID: 23805
		public bool everyFrame;
	}
}
