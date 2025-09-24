using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1A RID: 3098
	[ActionCategory("Math")]
	[Tooltip("Calculate the distance between two points and store it as a float.")]
	public class DistanceBetweenPoints2D : FsmStateAction
	{
		// Token: 0x06005E5A RID: 24154 RVA: 0x001DBE1B File Offset: 0x001DA01B
		public override void Reset()
		{
			this.distanceResult = null;
			this.point1 = null;
			this.point2 = null;
			this.everyFrame = false;
		}

		// Token: 0x06005E5B RID: 24155 RVA: 0x001DBE39 File Offset: 0x001DA039
		public override void OnEnter()
		{
			this.DoCalcDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x001DBE4F File Offset: 0x001DA04F
		public override void OnUpdate()
		{
			this.DoCalcDistance();
		}

		// Token: 0x06005E5D RID: 24157 RVA: 0x001DBE58 File Offset: 0x001DA058
		private void DoCalcDistance()
		{
			if (this.distanceResult == null)
			{
				return;
			}
			Vector2 a = new Vector2(this.point1.Value.x, this.point1.Value.y);
			Vector2 b = new Vector2(this.point2.Value.x, this.point2.Value.y);
			float value = Vector2.Distance(a, b);
			this.distanceResult.Value = value;
		}

		// Token: 0x04005AA9 RID: 23209
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat distanceResult;

		// Token: 0x04005AAA RID: 23210
		[RequiredField]
		public FsmVector2 point1;

		// Token: 0x04005AAB RID: 23211
		[RequiredField]
		public FsmVector2 point2;

		// Token: 0x04005AAC RID: 23212
		public bool everyFrame;
	}
}
