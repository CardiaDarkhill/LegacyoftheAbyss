using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C66 RID: 3174
	[ActionCategory("Math")]
	public class GetDifferenceBetweenFloats : FsmStateAction
	{
		// Token: 0x06005FED RID: 24557 RVA: 0x001E63AD File Offset: 0x001E45AD
		public override void Reset()
		{
			this.differenceResult = null;
			this.float1 = null;
			this.float2 = null;
			this.everyFrame = false;
		}

		// Token: 0x06005FEE RID: 24558 RVA: 0x001E63CB File Offset: 0x001E45CB
		public override void OnEnter()
		{
			this.DoCalcDifference();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FEF RID: 24559 RVA: 0x001E63E1 File Offset: 0x001E45E1
		public override void OnUpdate()
		{
			this.DoCalcDifference();
		}

		// Token: 0x06005FF0 RID: 24560 RVA: 0x001E63EC File Offset: 0x001E45EC
		private void DoCalcDifference()
		{
			if (this.differenceResult == null)
			{
				return;
			}
			float value = Mathf.Abs(this.float2.Value - this.float1.Value);
			this.differenceResult.Value = value;
		}

		// Token: 0x04005D40 RID: 23872
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat differenceResult;

		// Token: 0x04005D41 RID: 23873
		[RequiredField]
		public FsmFloat float1;

		// Token: 0x04005D42 RID: 23874
		[RequiredField]
		public FsmFloat float2;

		// Token: 0x04005D43 RID: 23875
		public bool everyFrame;
	}
}
