using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C58 RID: 3160
	[ActionCategory("Math")]
	[Tooltip("Get the angle between two vector3 positions. 0 is right, 90 is up etc.")]
	public class GetAngleBetweenPointsV2 : FsmStateAction
	{
		// Token: 0x06005FA9 RID: 24489 RVA: 0x001E574B File Offset: 0x001E394B
		public override void Reset()
		{
		}

		// Token: 0x06005FAA RID: 24490 RVA: 0x001E574D File Offset: 0x001E394D
		public override void OnEnter()
		{
			this.DoGetAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FAB RID: 24491 RVA: 0x001E5763 File Offset: 0x001E3963
		public override void OnUpdate()
		{
			this.DoGetAngle();
		}

		// Token: 0x06005FAC RID: 24492 RVA: 0x001E576C File Offset: 0x001E396C
		private void DoGetAngle()
		{
			float num = this.targetY.Value - this.originY.Value;
			float num2 = this.targetX.Value - this.originX.Value;
			float num3;
			for (num3 = Mathf.Atan2(num, num2) * 57.295776f; num3 < 0f; num3 += 360f)
			{
			}
			this.storeAngle.Value = num3;
		}

		// Token: 0x04005CFE RID: 23806
		public FsmFloat originX;

		// Token: 0x04005CFF RID: 23807
		public FsmFloat originY;

		// Token: 0x04005D00 RID: 23808
		public FsmFloat targetX;

		// Token: 0x04005D01 RID: 23809
		public FsmFloat targetY;

		// Token: 0x04005D02 RID: 23810
		public FsmFloat storeAngle;

		// Token: 0x04005D03 RID: 23811
		private FsmFloat x;

		// Token: 0x04005D04 RID: 23812
		private FsmFloat y;

		// Token: 0x04005D05 RID: 23813
		public bool everyFrame;
	}
}
