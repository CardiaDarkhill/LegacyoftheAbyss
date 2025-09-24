using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200134D RID: 4941
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV4 : YesNoAction
	{
		// Token: 0x06007FA5 RID: 32677 RVA: 0x0025C01E File Offset: 0x0025A21E
		public override void Reset()
		{
			base.Reset();
			this.Prompt = null;
			this.RequiredItem = null;
			this.RequiredAmount = null;
			this.ShowCounter = true;
			this.ConsumeItem = null;
			this.TakeDisplay = null;
			this.WillGetItem = null;
		}

		// Token: 0x06007FA6 RID: 32678 RVA: 0x0025C05C File Offset: 0x0025A25C
		protected override void DoOpen()
		{
			SavedItem requiredItem = this.RequiredItem.Value as SavedItem;
			Action yes = delegate()
			{
				ICollectableUIMsgItem collectableUIMsgItem = requiredItem as ICollectableUIMsgItem;
				if (collectableUIMsgItem != null)
				{
					TakeItemTypes takeItemType = (TakeItemTypes)this.TakeDisplay.Value;
					CollectableUIMsg.ShowTakeMsg(collectableUIMsgItem, takeItemType);
				}
				this.SendEvent(true);
			};
			Action no = delegate()
			{
				this.SendEvent(false);
			};
			LocalisedString s = this.Prompt;
			if (s.IsEmpty)
			{
				DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, requiredItem, this.RequiredAmount.Value, this.ShowCounter.Value, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem);
				return;
			}
			DialogueYesNoBox.Open(yes, no, this.ReturnHUDAfter.Value, s, requiredItem, this.RequiredAmount.Value, this.ShowCounter.Value, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem);
		}

		// Token: 0x06007FA7 RID: 32679 RVA: 0x0025C154 File Offset: 0x0025A354
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F29 RID: 32553
		public LocalisedFsmString Prompt;

		// Token: 0x04007F2A RID: 32554
		[ObjectType(typeof(SavedItem))]
		public FsmObject RequiredItem;

		// Token: 0x04007F2B RID: 32555
		public FsmInt RequiredAmount;

		// Token: 0x04007F2C RID: 32556
		public FsmBool ShowCounter;

		// Token: 0x04007F2D RID: 32557
		public FsmBool ConsumeItem;

		// Token: 0x04007F2E RID: 32558
		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;

		// Token: 0x04007F2F RID: 32559
		[ObjectType(typeof(SavedItem))]
		public FsmObject WillGetItem;
	}
}
