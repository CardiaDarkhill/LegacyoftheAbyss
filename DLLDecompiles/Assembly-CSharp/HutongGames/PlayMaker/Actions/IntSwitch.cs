using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F59 RID: 3929
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of an Integer Variable.")]
	public class IntSwitch : FsmStateAction
	{
		// Token: 0x06006D26 RID: 27942 RVA: 0x0021F7CE File Offset: 0x0021D9CE
		public override void Reset()
		{
			this.intVariable = null;
			this.compareTo = new FsmInt[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006D27 RID: 27943 RVA: 0x0021F7F6 File Offset: 0x0021D9F6
		public override void OnEnter()
		{
			this.DoIntSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D28 RID: 27944 RVA: 0x0021F80C File Offset: 0x0021DA0C
		public override void OnUpdate()
		{
			this.DoIntSwitch();
		}

		// Token: 0x06006D29 RID: 27945 RVA: 0x0021F814 File Offset: 0x0021DA14
		private void DoIntSwitch()
		{
			if (this.intVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (this.intVariable.Value == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04006CE9 RID: 27881
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer variable to test.")]
		public FsmInt intVariable;

		// Token: 0x04006CEA RID: 27882
		[CompoundArray("Int Switches", "Compare Int", "Send Event")]
		[Tooltip("The integer variable to test.")]
		public FsmInt[] compareTo;

		// Token: 0x04006CEB RID: 27883
		[Tooltip("Event to send if true.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04006CEC RID: 27884
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
