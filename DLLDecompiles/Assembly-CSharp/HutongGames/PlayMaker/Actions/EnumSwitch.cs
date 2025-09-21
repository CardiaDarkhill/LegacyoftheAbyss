using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F46 RID: 3910
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of an Enum Variable.")]
	public class EnumSwitch : FsmStateAction
	{
		// Token: 0x06006CC7 RID: 27847 RVA: 0x0021E8BB File Offset: 0x0021CABB
		public override void Reset()
		{
			this.enumVariable = null;
			this.compareTo = new FsmEnum[0];
			this.sendEvent = new FsmEvent[0];
			this.everyFrame = false;
		}

		// Token: 0x06006CC8 RID: 27848 RVA: 0x0021E8E3 File Offset: 0x0021CAE3
		public override void OnEnter()
		{
			this.DoEnumSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CC9 RID: 27849 RVA: 0x0021E8F9 File Offset: 0x0021CAF9
		public override void OnUpdate()
		{
			this.DoEnumSwitch();
		}

		// Token: 0x06006CCA RID: 27850 RVA: 0x0021E904 File Offset: 0x0021CB04
		private void DoEnumSwitch()
		{
			if (this.enumVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (object.Equals(this.enumVariable.Value, this.compareTo[i].Value))
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04006C80 RID: 27776
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Enum Variable to use.")]
		public FsmEnum enumVariable;

		// Token: 0x04006C81 RID: 27777
		[CompoundArray("Enum Switches", "Compare Enum Value", "Send Event")]
		[MatchFieldType("enumVariable")]
		[Tooltip("Compare Enum Values")]
		public FsmEnum[] compareTo;

		// Token: 0x04006C82 RID: 27778
		[Tooltip("Event to send if the Enum Variable value is equal.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04006C83 RID: 27779
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
