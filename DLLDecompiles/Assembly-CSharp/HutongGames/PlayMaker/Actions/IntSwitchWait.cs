using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C92 RID: 3218
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of an Integer Variable.")]
	public class IntSwitchWait : FsmStateAction
	{
		// Token: 0x060060B0 RID: 24752 RVA: 0x001EA526 File Offset: 0x001E8726
		public override void Reset()
		{
			this.time = null;
			this.intVariable = null;
			this.compareTo = new FsmInt[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x001EA555 File Offset: 0x001E8755
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x060060B2 RID: 24754 RVA: 0x001EA562 File Offset: 0x001E8762
		public override void OnUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.time.Value)
			{
				this.DoIntSwitch();
				base.Finish();
			}
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x001EA598 File Offset: 0x001E8798
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

		// Token: 0x04005E33 RID: 24115
		[RequiredField]
		[Tooltip("Time to wait in seconds.")]
		public FsmFloat time;

		// Token: 0x04005E34 RID: 24116
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer variable to test.")]
		public FsmInt intVariable;

		// Token: 0x04005E35 RID: 24117
		[CompoundArray("Int Switches", "Compare Int", "Send Event")]
		[Tooltip("The integer variable to test.")]
		public FsmInt[] compareTo;

		// Token: 0x04005E36 RID: 24118
		[Tooltip("Event to send if true.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04005E37 RID: 24119
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04005E38 RID: 24120
		private float timer;
	}
}
