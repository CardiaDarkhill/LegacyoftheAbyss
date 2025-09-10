using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001394 RID: 5012
	public sealed class UIMsgPopupSetMinY : FsmStateAction
	{
		// Token: 0x060080B2 RID: 32946 RVA: 0x0025EF35 File Offset: 0x0025D135
		public override void Reset()
		{
			this.value = null;
		}

		// Token: 0x060080B3 RID: 32947 RVA: 0x0025EF3E File Offset: 0x0025D13E
		public override void OnEnter()
		{
			UIMsgPopupBaseBase.MinYPos = this.value.Value;
			base.Finish();
		}

		// Token: 0x04007FF7 RID: 32759
		public FsmFloat value;
	}
}
