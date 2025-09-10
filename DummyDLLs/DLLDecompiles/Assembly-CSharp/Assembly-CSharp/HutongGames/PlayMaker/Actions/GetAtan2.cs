using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F2 RID: 4338
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Arc Tangent 2 as in atan2(y,x). You can get the result in degrees, simply check on the RadToDeg conversion")]
	public class GetAtan2 : FsmStateAction
	{
		// Token: 0x06007555 RID: 30037 RVA: 0x0023DCDF File Offset: 0x0023BEDF
		public override void Reset()
		{
			this.xValue = null;
			this.yValue = null;
			this.RadToDeg = true;
			this.everyFrame = false;
			this.angle = null;
		}

		// Token: 0x06007556 RID: 30038 RVA: 0x0023DD09 File Offset: 0x0023BF09
		public override void OnEnter()
		{
			this.DoATan();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007557 RID: 30039 RVA: 0x0023DD1F File Offset: 0x0023BF1F
		public override void OnUpdate()
		{
			this.DoATan();
		}

		// Token: 0x06007558 RID: 30040 RVA: 0x0023DD28 File Offset: 0x0023BF28
		private void DoATan()
		{
			float num = Mathf.Atan2(this.yValue.Value, this.xValue.Value);
			if (this.RadToDeg.Value)
			{
				num *= 57.29578f;
			}
			this.angle.Value = num;
		}

		// Token: 0x040075C3 RID: 30147
		[RequiredField]
		[Tooltip("The x value of the tan")]
		public FsmFloat xValue;

		// Token: 0x040075C4 RID: 30148
		[RequiredField]
		[Tooltip("The y value of the tan")]
		public FsmFloat yValue;

		// Token: 0x040075C5 RID: 30149
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
		public FsmFloat angle;

		// Token: 0x040075C6 RID: 30150
		[Tooltip("Check on if you want the angle expressed in degrees.")]
		public FsmBool RadToDeg;

		// Token: 0x040075C7 RID: 30151
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
