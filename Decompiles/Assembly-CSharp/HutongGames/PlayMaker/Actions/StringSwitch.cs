using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F61 RID: 3937
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of a String Variable.")]
	public class StringSwitch : FsmStateAction
	{
		// Token: 0x06006D4E RID: 27982 RVA: 0x0021FEE9 File Offset: 0x0021E0E9
		public override void Reset()
		{
			this.stringVariable = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006D4F RID: 27983 RVA: 0x0021FF11 File Offset: 0x0021E111
		public override void OnEnter()
		{
			this.DoStringSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D50 RID: 27984 RVA: 0x0021FF27 File Offset: 0x0021E127
		public override void OnUpdate()
		{
			this.DoStringSwitch();
		}

		// Token: 0x06006D51 RID: 27985 RVA: 0x0021FF30 File Offset: 0x0021E130
		private void DoStringSwitch()
		{
			if (this.stringVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (this.stringVariable.Value == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04006D11 RID: 27921
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to test.")]
		public FsmString stringVariable;

		// Token: 0x04006D12 RID: 27922
		[CompoundArray("String Switches", "Compare String", "Send Event")]
		[Tooltip("Compare to a string value.")]
		public FsmString[] compareTo;

		// Token: 0x04006D13 RID: 27923
		[Tooltip("Send this event if string matches.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04006D14 RID: 27924
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
