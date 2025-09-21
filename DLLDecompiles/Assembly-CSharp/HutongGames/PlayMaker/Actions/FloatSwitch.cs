using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F4A RID: 3914
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of a Float Variable. The float could represent distance, angle to a target, health left... The array sets up float ranges that correspond to Events.")]
	public class FloatSwitch : FsmStateAction
	{
		// Token: 0x06006CDC RID: 27868 RVA: 0x0021EC09 File Offset: 0x0021CE09
		public override void Reset()
		{
			this.floatVariable = null;
			this.lessThan = new FsmFloat[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006CDD RID: 27869 RVA: 0x0021EC31 File Offset: 0x0021CE31
		public override void OnEnter()
		{
			this.DoFloatSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CDE RID: 27870 RVA: 0x0021EC47 File Offset: 0x0021CE47
		public override void OnUpdate()
		{
			this.DoFloatSwitch();
		}

		// Token: 0x06006CDF RID: 27871 RVA: 0x0021EC50 File Offset: 0x0021CE50
		private void DoFloatSwitch()
		{
			if (this.floatVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.lessThan.Length; i++)
			{
				if (this.floatVariable.Value < this.lessThan[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04006C93 RID: 27795
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to test.")]
		public FsmFloat floatVariable;

		// Token: 0x04006C94 RID: 27796
		[CompoundArray("Float Switches", "Less Than", "Send Event")]
		[Tooltip("Test if the float is less than a value. Each entry in the array defines a range between it and the previous entry.")]
		public FsmFloat[] lessThan;

		// Token: 0x04006C95 RID: 27797
		[Tooltip("Event to send if true.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04006C96 RID: 27798
		[Tooltip("Repeat every frame. Useful if the variable is changing.")]
		public bool everyFrame;
	}
}
