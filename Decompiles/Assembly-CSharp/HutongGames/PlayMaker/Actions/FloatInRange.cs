using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4B RID: 3147
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Checks whether a float value is within a certain range (inclusive)")]
	public class FloatInRange : FsmStateAction
	{
		// Token: 0x06005F70 RID: 24432 RVA: 0x001E4EC7 File Offset: 0x001E30C7
		public override void Reset()
		{
			this.floatVariable = null;
			this.lowerValue = null;
			this.upperValue = null;
			this.boolVariable = null;
			this.everyFrame = false;
			this.trueEvent = null;
			this.falseEvent = null;
		}

		// Token: 0x06005F71 RID: 24433 RVA: 0x001E4EFA File Offset: 0x001E30FA
		public override void OnEnter()
		{
			this.DoFloatRange();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F72 RID: 24434 RVA: 0x001E4F10 File Offset: 0x001E3110
		public override void OnUpdate()
		{
			this.DoFloatRange();
		}

		// Token: 0x06005F73 RID: 24435 RVA: 0x001E4F18 File Offset: 0x001E3118
		private void DoFloatRange()
		{
			if (this.floatVariable.IsNone)
			{
				return;
			}
			if (this.floatVariable.Value <= this.upperValue.Value && this.floatVariable.Value >= this.lowerValue.Value)
			{
				this.boolVariable.Value = true;
				base.Fsm.Event(this.trueEvent);
				return;
			}
			this.boolVariable.Value = false;
			base.Fsm.Event(this.falseEvent);
		}

		// Token: 0x04005CCC RID: 23756
		[RequiredField]
		[Tooltip("The float variable to test.")]
		public FsmFloat floatVariable;

		// Token: 0x04005CCD RID: 23757
		[RequiredField]
		public FsmFloat lowerValue;

		// Token: 0x04005CCE RID: 23758
		[RequiredField]
		public FsmFloat upperValue;

		// Token: 0x04005CCF RID: 23759
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		// Token: 0x04005CD0 RID: 23760
		public FsmEvent trueEvent;

		// Token: 0x04005CD1 RID: 23761
		public FsmEvent falseEvent;

		// Token: 0x04005CD2 RID: 23762
		[Tooltip("Repeat every frame. Useful if the variable is changing.")]
		public bool everyFrame;
	}
}
