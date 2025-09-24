using System;
using System.Linq;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200134E RID: 4942
	[ActionCategory("Dialogue")]
	public class DialogueYesNoItemV5 : YesNoAction
	{
		// Token: 0x06007FA9 RID: 32681 RVA: 0x0025C164 File Offset: 0x0025A364
		public override void Reset()
		{
			base.Reset();
			this.Prompt = null;
			this.RequiredItems = null;
			this.RequiredAmounts = null;
			this.CurrencyCost = null;
			this.CurrencyType = null;
			this.ShowCounter = true;
			this.ConsumeItem = null;
			this.TakeDisplay = null;
			this.WillGetItem = null;
		}

		// Token: 0x06007FAA RID: 32682 RVA: 0x0025C1BC File Offset: 0x0025A3BC
		protected override void DoOpen()
		{
			LocalisedString s = this.Prompt;
			DialogueYesNoBox.Open(delegate()
			{
				base.SendEvent(true);
			}, delegate()
			{
				base.SendEvent(false);
			}, this.ReturnHUDAfter.Value, s.IsEmpty ? null : s, (CurrencyType)this.CurrencyType.Value, this.CurrencyCost.Value, this.RequiredItems.objectReferences.Cast<SavedItem>().ToList<SavedItem>(), this.RequiredAmounts.intValues, this.ShowCounter.Value, this.ConsumeItem.Value, this.WillGetItem.Value as SavedItem, (TakeItemTypes)this.TakeDisplay.Value);
		}

		// Token: 0x06007FAB RID: 32683 RVA: 0x0025C280 File Offset: 0x0025A480
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007F30 RID: 32560
		public LocalisedFsmString Prompt;

		// Token: 0x04007F31 RID: 32561
		[ArrayEditor(typeof(SavedItem), "", 0, 0, 65536)]
		public FsmArray RequiredItems;

		// Token: 0x04007F32 RID: 32562
		[ArrayEditor(VariableType.Int, "", 0, 0, 65536)]
		public FsmArray RequiredAmounts;

		// Token: 0x04007F33 RID: 32563
		public FsmInt CurrencyCost;

		// Token: 0x04007F34 RID: 32564
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		// Token: 0x04007F35 RID: 32565
		public FsmBool ShowCounter;

		// Token: 0x04007F36 RID: 32566
		public FsmBool ConsumeItem;

		// Token: 0x04007F37 RID: 32567
		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;

		// Token: 0x04007F38 RID: 32568
		[ObjectType(typeof(SavedItem))]
		public FsmObject WillGetItem;
	}
}
