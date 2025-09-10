using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200128B RID: 4747
	[ActionCategory("Dialogue")]
	public class RelicBoardOwnerYesNo : YesNoAction
	{
		// Token: 0x06007CC8 RID: 31944 RVA: 0x00254646 File Offset: 0x00252846
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
			this.TranslationSheet = null;
			this.TranslationKey = null;
			this.TranslationKeyPlural = null;
		}

		// Token: 0x06007CC9 RID: 31945 RVA: 0x0025466C File Offset: 0x0025286C
		protected override void DoOpen()
		{
			RelicBoardOwner component = this.Target.GetSafe(this).GetComponent<RelicBoardOwner>();
			if (this.relics == null)
			{
				this.relics = new List<CollectableItemRelicType>();
			}
			this.relics.Clear();
			this.relics.AddRange((from relic in component.GetRelicsToDeposit()
			select relic.RelicType).Distinct<CollectableItemRelicType>());
			if (this.amounts == null)
			{
				this.amounts = new List<int>();
			}
			this.amounts.Clear();
			this.amounts.AddRange(from type in this.relics
			select type.CollectedAmount);
			string text = new LocalisedString(this.TranslationSheet.Value, (this.relics.Count > 1) ? this.TranslationKeyPlural.Value : this.TranslationKey.Value);
			DialogueYesNoBox.Open(delegate()
			{
				base.SendEvent(true);
			}, delegate()
			{
				base.SendEvent(false);
			}, this.ReturnHUDAfter.Value, text, this.relics, this.amounts, false, false, null);
		}

		// Token: 0x06007CCA RID: 31946 RVA: 0x002547AA File Offset: 0x002529AA
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x04007CE0 RID: 31968
		public FsmOwnerDefault Target;

		// Token: 0x04007CE1 RID: 31969
		public FsmString TranslationSheet;

		// Token: 0x04007CE2 RID: 31970
		public FsmString TranslationKey;

		// Token: 0x04007CE3 RID: 31971
		public FsmString TranslationKeyPlural;

		// Token: 0x04007CE4 RID: 31972
		private List<CollectableItemRelicType> relics;

		// Token: 0x04007CE5 RID: 31973
		private List<int> amounts;
	}
}
