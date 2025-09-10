using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F49 RID: 3913
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the sign of a Float (positive or negative).")]
	public class FloatSignTest : FsmStateAction
	{
		// Token: 0x06006CD6 RID: 27862 RVA: 0x0021EB5F File Offset: 0x0021CD5F
		public override void Reset()
		{
			this.floatValue = 0f;
			this.isPositive = null;
			this.isNegative = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CD7 RID: 27863 RVA: 0x0021EB86 File Offset: 0x0021CD86
		public override void OnEnter()
		{
			this.DoSignTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CD8 RID: 27864 RVA: 0x0021EB9C File Offset: 0x0021CD9C
		public override void OnUpdate()
		{
			this.DoSignTest();
		}

		// Token: 0x06006CD9 RID: 27865 RVA: 0x0021EBA4 File Offset: 0x0021CDA4
		private void DoSignTest()
		{
			if (this.floatValue == null)
			{
				return;
			}
			base.Fsm.Event((this.floatValue.Value < 0f) ? this.isNegative : this.isPositive);
		}

		// Token: 0x06006CDA RID: 27866 RVA: 0x0021EBDA File Offset: 0x0021CDDA
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.isPositive) && FsmEvent.IsNullOrEmpty(this.isNegative))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006C8F RID: 27791
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to test.")]
		public FsmFloat floatValue;

		// Token: 0x04006C90 RID: 27792
		[Tooltip("Event to send if the float variable is positive.")]
		public FsmEvent isPositive;

		// Token: 0x04006C91 RID: 27793
		[Tooltip("Event to send if the float variable is negative.")]
		public FsmEvent isNegative;

		// Token: 0x04006C92 RID: 27794
		[Tooltip("Repeat every frame. Useful if you want to wait until a float is positive/negative.")]
		public bool everyFrame;
	}
}
