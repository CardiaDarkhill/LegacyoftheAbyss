using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001348 RID: 4936
	[ActionCategory("Dialogue")]
	public class DialogueYesNoV2 : YesNoAction
	{
		// Token: 0x06007F84 RID: 32644 RVA: 0x0025B9A3 File Offset: 0x00259BA3
		public bool IsNotUsingCurrency()
		{
			return !this.UseCurrency.Value;
		}

		// Token: 0x06007F85 RID: 32645 RVA: 0x0025B9B3 File Offset: 0x00259BB3
		public override void Reset()
		{
			base.Reset();
			this.TranslationSheet = null;
			this.TranslationKey = null;
			this.UseCurrency = null;
			this.CurrencyCost = null;
			this.CurrencyType = null;
			this.ConsumeCurrency = null;
			this.WillGetItem = null;
		}

		// Token: 0x06007F86 RID: 32646 RVA: 0x0025B9EC File Offset: 0x00259BEC
		protected override void DoOpen()
		{
			string text = new LocalisedString(this.TranslationSheet.Value, this.TranslationKey.Value);
			Action yes = delegate()
			{
				base.SendEvent(true);
			};
			Action no = delegate()
			{
				base.SendEvent(false);
			};
			if (this.IsNotUsingCurrency())
			{
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, this.WillGetItem.Value as CollectableItem);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, (CurrencyType)this.CurrencyType.Value, this.CurrencyCost.Value, true, this.ConsumeCurrency.Value, this.WillGetItem.Value as CollectableItem);
		}

		// Token: 0x06007F87 RID: 32647 RVA: 0x0025BAAA File Offset: 0x00259CAA
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F08 RID: 32520
		public FsmString TranslationSheet;

		// Token: 0x04007F09 RID: 32521
		public FsmString TranslationKey;

		// Token: 0x04007F0A RID: 32522
		public FsmBool UseCurrency;

		// Token: 0x04007F0B RID: 32523
		[HideIf("IsNotUsingCurrency")]
		public FsmInt CurrencyCost;

		// Token: 0x04007F0C RID: 32524
		[HideIf("IsNotUsingCurrency")]
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007F0D RID: 32525
		[HideIf("IsNotUsingCurrency")]
		public FsmBool ConsumeCurrency;

		// Token: 0x04007F0E RID: 32526
		[ObjectType(typeof(CollectableItem))]
		public FsmObject WillGetItem;
	}
}
