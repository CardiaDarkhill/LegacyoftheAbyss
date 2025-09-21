using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFB RID: 3067
	[ActionCategory("Math")]
	[Tooltip("Calculate the distance between two points and compare it against a known distance value.")]
	public class CompareDistance2D : FsmStateAction
	{
		// Token: 0x06005DC9 RID: 24009 RVA: 0x001D957B File Offset: 0x001D777B
		public override void Reset()
		{
			this.point1 = null;
			this.point2 = null;
			this.knownDistance = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DCA RID: 24010 RVA: 0x001D9599 File Offset: 0x001D7799
		public override void OnEnter()
		{
			this.sqrDistanceTest = this.knownDistance.Value * this.knownDistance.Value;
			this.DoCompareDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DCB RID: 24011 RVA: 0x001D95CC File Offset: 0x001D77CC
		public override void OnUpdate()
		{
			this.DoCompareDistance();
		}

		// Token: 0x06005DCC RID: 24012 RVA: 0x001D95D4 File Offset: 0x001D77D4
		private void DoCalcDistance()
		{
		}

		// Token: 0x06005DCD RID: 24013 RVA: 0x001D95D8 File Offset: 0x001D77D8
		private void DoCompareDistance()
		{
			Vector2 a = new Vector2(this.point1.Value.x, this.point1.Value.y);
			Vector2 b = new Vector2(this.point2.Value.x, this.point2.Value.y);
			float magnitude = (a - b).magnitude;
			float value = this.knownDistance.Value;
		}

		// Token: 0x04005A1F RID: 23071
		[RequiredField]
		public FsmVector2 point1;

		// Token: 0x04005A20 RID: 23072
		[RequiredField]
		public FsmVector2 point2;

		// Token: 0x04005A21 RID: 23073
		[RequiredField]
		public FsmFloat knownDistance;

		// Token: 0x04005A22 RID: 23074
		public bool everyFrame;

		// Token: 0x04005A23 RID: 23075
		private float sqrDistanceTest;
	}
}
