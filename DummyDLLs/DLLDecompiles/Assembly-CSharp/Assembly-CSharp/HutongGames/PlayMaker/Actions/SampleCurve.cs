using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F88 RID: 3976
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Gets the value of a curve at a given time and stores it in a Float Variable. NOTE: This can be used for more than just animation! It's a general way to transform an input number into an output number using a curve (e.g., linear input -> bell curve).")]
	public class SampleCurve : FsmStateAction
	{
		// Token: 0x06006DFF RID: 28159 RVA: 0x00221DEA File Offset: 0x0021FFEA
		public override void Reset()
		{
			this.curve = null;
			this.sampleAt = null;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E00 RID: 28160 RVA: 0x00221E08 File Offset: 0x00220008
		public override void OnEnter()
		{
			this.DoSampleCurve();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E01 RID: 28161 RVA: 0x00221E1E File Offset: 0x0022001E
		public override void OnUpdate()
		{
			this.DoSampleCurve();
		}

		// Token: 0x06006E02 RID: 28162 RVA: 0x00221E28 File Offset: 0x00220028
		private void DoSampleCurve()
		{
			if (this.curve == null || this.curve.curve == null || this.storeValue == null)
			{
				return;
			}
			this.storeValue.Value = this.curve.curve.Evaluate(this.sampleAt.Value);
		}

		// Token: 0x04006DB2 RID: 28082
		[RequiredField]
		[Tooltip("Click to edit the curve.")]
		public FsmAnimationCurve curve;

		// Token: 0x04006DB3 RID: 28083
		[RequiredField]
		[Tooltip("Sample the curve at this point.")]
		public FsmFloat sampleAt;

		// Token: 0x04006DB4 RID: 28084
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the sampled value in a float variable.")]
		public FsmFloat storeValue;

		// Token: 0x04006DB5 RID: 28085
		[Tooltip("Do it every frame. Useful if Sample At is changing.")]
		public bool everyFrame;
	}
}
