using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200134A RID: 4938
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItem : YesNoAction
	{
		// Token: 0x06007F93 RID: 32659 RVA: 0x0025BC47 File Offset: 0x00259E47
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

		// Token: 0x06007F94 RID: 32660 RVA: 0x0025BC7C File Offset: 0x00259E7C
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
			string text = new LocalisedString(this.TranslationSheet.Value, this.TranslationKey.Value);
			bool flag;
			try
			{
				string.Format(text, "TEST");
				flag = true;
			}
			catch (FormatException)
			{
				flag = false;
			}
			if (flag)
			{
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, this.RequiredItem.Value as CollectableItem, this.RequiredAmount.Value, true, this.ConsumeItem.Value, this.WillGetItem.Value as CollectableItem);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, text, this.RequiredItem.Value as CollectableItem, this.RequiredAmount.Value, true, this.ConsumeItem.Value, this.WillGetItem.Value as CollectableItem);
		}

		// Token: 0x06007F95 RID: 32661 RVA: 0x0025BD80 File Offset: 0x00259F80
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F17 RID: 32535
		public FsmString TranslationSheet;

		// Token: 0x04007F18 RID: 32536
		public FsmString TranslationKey;

		// Token: 0x04007F19 RID: 32537
		[ObjectType(typeof(CollectableItem))]
		public FsmObject RequiredItem;

		// Token: 0x04007F1A RID: 32538
		public FsmInt RequiredAmount;

		// Token: 0x04007F1B RID: 32539
		public FsmBool ConsumeItem;

		// Token: 0x04007F1C RID: 32540
		[ObjectType(typeof(CollectableItem))]
		public FsmObject WillGetItem;
	}
}
