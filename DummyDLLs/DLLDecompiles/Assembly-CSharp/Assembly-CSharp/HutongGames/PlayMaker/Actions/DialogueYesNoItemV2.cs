using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200134B RID: 4939
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV2 : YesNoAction
	{
		// Token: 0x06007F99 RID: 32665 RVA: 0x0025BDA1 File Offset: 0x00259FA1
		public override void Reset()
		{
			base.Reset();
			this.TranslationSheet = null;
			this.TranslationKey = null;
			this.RequiredItem = null;
			this.RequiredAmount = null;
			this.ConsumeItem = null;
			this.WillGetItem = null;
		}

		// Token: 0x06007F9A RID: 32666 RVA: 0x0025BDD4 File Offset: 0x00259FD4
		protected override void DoOpen()
		{
			Action yes = delegate()
			{
				base.SendEvent(true);
			};
			Action no = delegate()
			{
				base.SendEvent(false);
			};
			LocalisedString s = new LocalisedString(this.TranslationSheet.Value, this.TranslationKey.Value);
			if (s.IsEmpty)
			{
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, this.RequiredItem.Value as SavedItem, this.RequiredAmount.Value, true, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, s, this.RequiredItem.Value as SavedItem, this.RequiredAmount.Value, true, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem);
		}

		// Token: 0x06007F9B RID: 32667 RVA: 0x0025BEBA File Offset: 0x0025A0BA
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F1D RID: 32541
		public FsmString TranslationSheet;

		// Token: 0x04007F1E RID: 32542
		public FsmString TranslationKey;

		// Token: 0x04007F1F RID: 32543
		[ObjectType(typeof(SavedItem))]
		public FsmObject RequiredItem;

		// Token: 0x04007F20 RID: 32544
		public FsmInt RequiredAmount;

		// Token: 0x04007F21 RID: 32545
		public FsmBool ConsumeItem;

		// Token: 0x04007F22 RID: 32546
		[ObjectType(typeof(SavedItem))]
		public FsmObject WillGetItem;
	}
}
