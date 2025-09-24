using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001349 RID: 4937
	[ActionCategory("Dialogue")]
	public class DialogueYesNoV3 : YesNoAction
	{
		// Token: 0x06007F8B RID: 32651 RVA: 0x0025BACB File Offset: 0x00259CCB
		public bool IsNotUsingCurrency()
		{
			return !this.UseCurrency.Value;
		}

		// Token: 0x06007F8C RID: 32652 RVA: 0x0025BADB File Offset: 0x00259CDB
		public bool IsUsingLiteralText()
		{
			return !this.Text.IsNone;
		}

		// Token: 0x06007F8D RID: 32653 RVA: 0x0025BAEC File Offset: 0x00259CEC
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
			this.ConsumeCurrency = null;
			this.WillGetItem = null;
		}

		// Token: 0x06007F8E RID: 32654 RVA: 0x0025BB44 File Offset: 0x00259D44
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
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, this.WillGetItem.Value as CollectableItem);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, (CurrencyType)this.CurrencyType.Value, this.CurrencyCost.Value, true, this.ConsumeCurrency.Value, this.WillGetItem.Value as CollectableItem);
		}

		// Token: 0x06007F8F RID: 32655 RVA: 0x0025BC26 File Offset: 0x00259E26
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F0F RID: 32527
		public FsmString Text;

		// Token: 0x04007F10 RID: 32528
		[HideIf("IsUsingLiteralText")]
		public FsmString TranslationSheet;

		// Token: 0x04007F11 RID: 32529
		[HideIf("IsUsingLiteralText")]
		public FsmString TranslationKey;

		// Token: 0x04007F12 RID: 32530
		public FsmBool UseCurrency;

		// Token: 0x04007F13 RID: 32531
		[HideIf("IsNotUsingCurrency")]
		public FsmInt CurrencyCost;

		// Token: 0x04007F14 RID: 32532
		[HideIf("IsNotUsingCurrency")]
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007F15 RID: 32533
		[HideIf("IsNotUsingCurrency")]
		public FsmBool ConsumeCurrency;

		// Token: 0x04007F16 RID: 32534
		[ObjectType(typeof(CollectableItem))]
		public FsmObject WillGetItem;
	}
}
