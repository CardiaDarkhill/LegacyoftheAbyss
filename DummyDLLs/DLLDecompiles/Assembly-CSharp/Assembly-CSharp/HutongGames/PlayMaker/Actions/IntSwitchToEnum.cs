using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C91 RID: 3217
	public class IntSwitchToEnum : FsmStateAction
	{
		// Token: 0x060060AB RID: 24747 RVA: 0x001EA489 File Offset: 0x001E8689
		public override void Reset()
		{
			this.Value = null;
			this.From = null;
			this.To = null;
			this.StoreValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x060060AC RID: 24748 RVA: 0x001EA4AE File Offset: 0x001E86AE
		public override void OnEnter()
		{
			this.DoEnumSwitch();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060060AD RID: 24749 RVA: 0x001EA4C4 File Offset: 0x001E86C4
		public override void OnUpdate()
		{
			this.DoEnumSwitch();
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x001EA4CC File Offset: 0x001E86CC
		private void DoEnumSwitch()
		{
			for (int i = 0; i < this.From.Length; i++)
			{
				if (this.Value.Value == this.From[i].Value)
				{
					this.StoreValue.Value = this.To[i].Value;
				}
			}
		}

		// Token: 0x04005E2E RID: 24110
		[RequiredField]
		public FsmInt Value;

		// Token: 0x04005E2F RID: 24111
		[CompoundArray("Switches", "From Int", "To Enum")]
		public FsmInt[] From;

		// Token: 0x04005E30 RID: 24112
		[MatchFieldType("StoreValue")]
		public FsmEnum[] To;

		// Token: 0x04005E31 RID: 24113
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreValue;

		// Token: 0x04005E32 RID: 24114
		public bool EveryFrame;
	}
}
