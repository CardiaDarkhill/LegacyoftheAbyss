using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001344 RID: 4932
	public class CurrencyCounterMethod : FsmStateAction
	{
		// Token: 0x06007F73 RID: 32627 RVA: 0x0025B6E4 File Offset: 0x002598E4
		public bool NotRequiresAmount()
		{
			if (this.Method.IsNone)
			{
				return true;
			}
			CurrencyCounterMethod.Methods methods = (CurrencyCounterMethod.Methods)this.Method.Value;
			return methods - CurrencyCounterMethod.Methods.Take > 1;
		}

		// Token: 0x06007F74 RID: 32628 RVA: 0x0025B71A File Offset: 0x0025991A
		public override void Reset()
		{
			this.CurrencyType = null;
			this.Method = CurrencyCounterMethod.Methods.ToZero;
			this.Amount = null;
		}

		// Token: 0x06007F75 RID: 32629 RVA: 0x0025B73C File Offset: 0x0025993C
		public override void OnEnter()
		{
			if (!this.CurrencyType.IsNone && !this.Method.IsNone)
			{
				CurrencyCounterMethod.Methods methods = (CurrencyCounterMethod.Methods)this.Method.Value;
				CurrencyType type = (CurrencyType)this.CurrencyType.Value;
				switch (methods)
				{
				case CurrencyCounterMethod.Methods.ToZero:
					CurrencyCounter.ToZero(type);
					break;
				case CurrencyCounterMethod.Methods.Take:
					CurrencyCounter.Take(this.Amount.Value, type);
					break;
				case CurrencyCounterMethod.Methods.Add:
					CurrencyCounter.Add(this.Amount.Value, type);
					break;
				case CurrencyCounterMethod.Methods.Show:
					CurrencyCounter.Show(type, false);
					break;
				case CurrencyCounterMethod.Methods.Hide:
					CurrencyCounter.Hide(type);
					break;
				}
			}
			base.Finish();
		}

		// Token: 0x04007EFD RID: 32509
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007EFE RID: 32510
		[ObjectType(typeof(CurrencyCounterMethod.Methods))]
		public FsmEnum Method;

		// Token: 0x04007EFF RID: 32511
		[HideIf("NotRequiresAmount")]
		public FsmInt Amount;

		// Token: 0x02001BF6 RID: 7158
		public enum Methods
		{
			// Token: 0x04009FB2 RID: 40882
			ToZero,
			// Token: 0x04009FB3 RID: 40883
			Take,
			// Token: 0x04009FB4 RID: 40884
			Add,
			// Token: 0x04009FB5 RID: 40885
			Show,
			// Token: 0x04009FB6 RID: 40886
			Hide
		}
	}
}
