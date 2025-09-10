using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001240 RID: 4672
	public class TakeCurrency : FsmStateAction
	{
		// Token: 0x06007BA0 RID: 31648 RVA: 0x00250103 File Offset: 0x0024E303
		public override void Reset()
		{
			this.CurrencyType = null;
			this.Amount = null;
		}

		// Token: 0x06007BA1 RID: 31649 RVA: 0x00250114 File Offset: 0x0024E314
		public override void OnEnter()
		{
			if (!this.CurrencyType.IsNone && this.Amount.Value > 0)
			{
				CurrencyManager.TakeCurrency(this.Amount.Value, (CurrencyType)this.CurrencyType.Value, true);
			}
			base.Finish();
		}

		// Token: 0x04007BDA RID: 31706
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007BDB RID: 31707
		public FsmInt Amount;
	}
}
