using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001241 RID: 4673
	public class AddCurrency : FsmStateAction
	{
		// Token: 0x06007BA3 RID: 31651 RVA: 0x0025016B File Offset: 0x0024E36B
		public override void Reset()
		{
			this.Target = null;
			this.CurrencyType = null;
			this.Amount = null;
		}

		// Token: 0x06007BA4 RID: 31652 RVA: 0x00250184 File Offset: 0x0024E384
		public override void OnEnter()
		{
			if (!this.CurrencyType.IsNone && this.Amount.Value > 0)
			{
				CurrencyManager.AddCurrency(this.Amount.Value, (CurrencyType)this.CurrencyType.Value, true);
			}
			base.Finish();
		}

		// Token: 0x04007BDC RID: 31708
		public FsmOwnerDefault Target;

		// Token: 0x04007BDD RID: 31709
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007BDE RID: 31710
		public FsmInt Amount;
	}
}
