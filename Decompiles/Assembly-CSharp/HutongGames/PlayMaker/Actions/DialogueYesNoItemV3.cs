using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200134C RID: 4940
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV3 : YesNoAction
	{
		// Token: 0x06007F9F RID: 32671 RVA: 0x0025BEDB File Offset: 0x0025A0DB
		public override void Reset()
		{
			base.Reset();
			this.Prompt = null;
			this.RequiredItem = null;
			this.RequiredAmount = null;
			this.ShowCounter = true;
			this.ConsumeItem = null;
			this.WillGetItem = null;
		}

		// Token: 0x06007FA0 RID: 32672 RVA: 0x0025BF14 File Offset: 0x0025A114
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
			LocalisedString s = this.Prompt;
			if (s.IsEmpty)
			{
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, this.RequiredItem.Value as SavedItem, this.RequiredAmount.Value, this.ShowCounter.Value, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, s, this.RequiredItem.Value as SavedItem, this.RequiredAmount.Value, this.ShowCounter.Value, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem);
		}

		// Token: 0x06007FA1 RID: 32673 RVA: 0x0025BFFD File Offset: 0x0025A1FD
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F23 RID: 32547
		public LocalisedFsmString Prompt;

		// Token: 0x04007F24 RID: 32548
		[ObjectType(typeof(SavedItem))]
		public FsmObject RequiredItem;

		// Token: 0x04007F25 RID: 32549
		public FsmInt RequiredAmount;

		// Token: 0x04007F26 RID: 32550
		public FsmBool ShowCounter;

		// Token: 0x04007F27 RID: 32551
		public FsmBool ConsumeItem;

		// Token: 0x04007F28 RID: 32552
		[ObjectType(typeof(SavedItem))]
		public FsmObject WillGetItem;
	}
}
