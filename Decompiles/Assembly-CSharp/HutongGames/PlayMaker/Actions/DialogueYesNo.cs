using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001347 RID: 4935
	[ActionCategory("Dialogue")]
	public class DialogueYesNo : YesNoAction
	{
		// Token: 0x06007F7C RID: 32636 RVA: 0x0025B868 File Offset: 0x00259A68
		public bool IsNotUsingCurrency()
		{
			return !this.UseCurrency.Value;
		}

		// Token: 0x06007F7D RID: 32637 RVA: 0x0025B878 File Offset: 0x00259A78
		public bool IsUsingLiteralText()
		{
			return !this.Text.IsNone;
		}

		// Token: 0x06007F7E RID: 32638 RVA: 0x0025B888 File Offset: 0x00259A88
		public override void Reset()
		{
			base.Reset();
			this.Text = new FsmString
			{
				UseVariable = true
			};
			this.TranslationSheet = null;
			this.TranslationKey = null;
			this.UseCurrency = null;
			this.CurrencyCost = null;
			this.CurrencyType = null;
		}

		// Token: 0x06007F7F RID: 32639 RVA: 0x0025B8C8 File Offset: 0x00259AC8
		protected override void DoOpen()
		{
			string text = this.IsUsingLiteralText() ? this.Text.Value.Replace("<br>", "\n") : new LocalisedString(this.TranslationSheet.Value, this.TranslationKey.Value);
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
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, null);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, (CurrencyType)this.CurrencyType.Value, this.CurrencyCost.Value, true, true, null);
		}

		// Token: 0x06007F80 RID: 32640 RVA: 0x0025B982 File Offset: 0x00259B82
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F02 RID: 32514
		public FsmString Text;

		// Token: 0x04007F03 RID: 32515
		[HideIf("IsUsingLiteralText")]
		public FsmString TranslationSheet;

		// Token: 0x04007F04 RID: 32516
		[HideIf("IsUsingLiteralText")]
		public FsmString TranslationKey;

		// Token: 0x04007F05 RID: 32517
		public FsmBool UseCurrency;

		// Token: 0x04007F06 RID: 32518
		[HideIf("IsNotUsingCurrency")]
		public FsmInt CurrencyCost;

		// Token: 0x04007F07 RID: 32519
		[HideIf("IsNotUsingCurrency")]
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;
	}
}
