using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C29 RID: 3113
	public class EnumBitmaskSwitch : FsmStateAction
	{
		// Token: 0x06005EC1 RID: 24257 RVA: 0x001DFE81 File Offset: 0x001DE081
		public override void Reset()
		{
			this.EnumVariable = null;
			this.Switches = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005EC2 RID: 24258 RVA: 0x001DFE98 File Offset: 0x001DE098
		public override void OnEnter()
		{
			this.DoEnumSwitch();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005EC3 RID: 24259 RVA: 0x001DFEAE File Offset: 0x001DE0AE
		public override void OnUpdate()
		{
			this.DoEnumSwitch();
		}

		// Token: 0x06005EC4 RID: 24260 RVA: 0x001DFEB8 File Offset: 0x001DE0B8
		private void DoEnumSwitch()
		{
			if (this.EnumVariable.IsNone)
			{
				return;
			}
			int num = (int)this.EnumVariable.RawValue;
			foreach (EnumBitmaskSwitch.SwitchItem switchItem in this.Switches)
			{
				int num2 = 0;
				foreach (FsmEnum fsmEnum in switchItem.Values)
				{
					num2 |= (int)fsmEnum.RawValue;
				}
				if ((num & num2) == num2)
				{
					base.Fsm.Event(switchItem.SendEvent);
					return;
				}
			}
		}

		// Token: 0x04005B66 RID: 23398
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmEnum EnumVariable;

		// Token: 0x04005B67 RID: 23399
		public EnumBitmaskSwitch.SwitchItem[] Switches;

		// Token: 0x04005B68 RID: 23400
		public bool EveryFrame;

		// Token: 0x02001B81 RID: 7041
		[Serializable]
		public class SwitchItem
		{
			// Token: 0x04009D6E RID: 40302
			[MatchFieldType("EnumVariable")]
			public FsmEnum[] Values;

			// Token: 0x04009D6F RID: 40303
			public FsmEvent SendEvent;
		}
	}
}
