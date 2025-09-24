using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C90 RID: 3216
	public class IntSwitchToArray : FsmStateAction
	{
		// Token: 0x060060A6 RID: 24742 RVA: 0x001EA3F1 File Offset: 0x001E85F1
		public override void Reset()
		{
			this.Value = null;
			this.From = null;
			this.To = null;
			this.StoreValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x001EA416 File Offset: 0x001E8616
		public override void OnEnter()
		{
			this.DoEnumSwitch();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x001EA42C File Offset: 0x001E862C
		public override void OnUpdate()
		{
			this.DoEnumSwitch();
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x001EA434 File Offset: 0x001E8634
		private void DoEnumSwitch()
		{
			for (int i = 0; i < this.From.Length; i++)
			{
				if (this.Value.Value == this.From[i].Value)
				{
					this.StoreValue.CopyValues(this.To[i]);
				}
			}
		}

		// Token: 0x04005E29 RID: 24105
		[RequiredField]
		public FsmInt Value;

		// Token: 0x04005E2A RID: 24106
		[CompoundArray("Switches", "From Int", "To FsmArray")]
		public FsmInt[] From;

		// Token: 0x04005E2B RID: 24107
		[UIHint(UIHint.Variable)]
		public FsmArray[] To;

		// Token: 0x04005E2C RID: 24108
		[UIHint(UIHint.Variable)]
		public FsmArray StoreValue;

		// Token: 0x04005E2D RID: 24109
		public bool EveryFrame;
	}
}
