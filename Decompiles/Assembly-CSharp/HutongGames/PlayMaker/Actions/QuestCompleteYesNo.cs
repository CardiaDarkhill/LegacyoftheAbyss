using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200134F RID: 4943
	[ActionCategory("Dialogue")]
	public class QuestCompleteYesNo : YesNoAction
	{
		// Token: 0x06007FAF RID: 32687 RVA: 0x0025C2A1 File Offset: 0x0025A4A1
		public override void Reset()
		{
			base.Reset();
			this.Quest = null;
			this.ConsumeCurrency = null;
		}

		// Token: 0x06007FB0 RID: 32688 RVA: 0x0025C2B8 File Offset: 0x0025A4B8
		protected override void DoOpen()
		{
			FullQuestBase fullQuestBase = this.Quest.Value as FullQuestBase;
			if (fullQuestBase == null)
			{
				return;
			}
			bool canComplete = fullQuestBase.CanComplete;
			bool consumeCurrency = this.ConsumeCurrency.Value && fullQuestBase.ConsumeTargetIfApplicable;
			SavedItem savedItem = null;
			QuestTargetCurrency questTargetCurrency = null;
			int amount = 0;
			int num = 0;
			foreach (FullQuestBase.QuestTarget questTarget in fullQuestBase.Targets)
			{
				SavedItem counter = questTarget.Counter;
				QuestTargetCurrency questTargetCurrency2 = questTarget.Counter as QuestTargetCurrency;
				if (questTarget.Count > 0 && (!(counter == null) || !(questTargetCurrency2 == null)))
				{
					num++;
					if (num <= 1)
					{
						savedItem = counter;
						questTargetCurrency = questTargetCurrency2;
						amount = questTarget.Count;
					}
				}
			}
			if (num == 0)
			{
				this.<DoOpen>g__NoTarget|3_0();
				return;
			}
			if (num != 1)
			{
				List<QuestTargetCounter> items = (from target in fullQuestBase.Targets
				select target.Counter).ToList<QuestTargetCounter>();
				List<int> amounts = (from target in fullQuestBase.Targets
				select target.Count).ToList<int>();
				string text;
				if (fullQuestBase.GiveNameOverride.IsEmpty)
				{
					text = Language.Get("GIVE_ITEMS_PROMPT", "UI");
				}
				else
				{
					text = Language.Get("GIVE_ITEM_PROMPT", "UI");
					text = string.Format(text, fullQuestBase.GiveNameOverride);
				}
				DialogueYesNoBox.Open(new Action(this.<DoOpen>g__TrueWrapper|3_2), new Action(this.<DoOpen>g__FalseWrapper|3_1), this.ReturnHUDAfter.Value, text, items, amounts, false, consumeCurrency, null);
				return;
			}
			if (questTargetCurrency != null)
			{
				DialogueYesNoBox.Open(new Action(this.<DoOpen>g__TrueWrapper|3_2), new Action(this.<DoOpen>g__FalseWrapper|3_1), this.ReturnHUDAfter.Value, questTargetCurrency.GivePromptText, questTargetCurrency.CurrencyType, amount, true, consumeCurrency, null);
				return;
			}
			if (savedItem)
			{
				DialogueYesNoBox.Open(new Action(this.<DoOpen>g__TrueWrapper|3_2), new Action(this.<DoOpen>g__FalseWrapper|3_1), this.ReturnHUDAfter.Value, savedItem, amount, false, consumeCurrency, null);
				return;
			}
			this.<DoOpen>g__NoTarget|3_0();
		}

		// Token: 0x06007FB1 RID: 32689 RVA: 0x0025C508 File Offset: 0x0025A708
		protected override void DoForceClose()
		{
			DialogueYesNoBox.ForceClose();
		}

		// Token: 0x06007FB3 RID: 32691 RVA: 0x0025C517 File Offset: 0x0025A717
		[CompilerGenerated]
		private void <DoOpen>g__NoTarget|3_0()
		{
			DialogueYesNoBox.Open(new Action(this.<DoOpen>g__TrueWrapper|3_2), new Action(this.<DoOpen>g__FalseWrapper|3_1), this.ReturnHUDAfter.Value, "! Turn in Quest? !", null);
		}

		// Token: 0x06007FB4 RID: 32692 RVA: 0x0025C547 File Offset: 0x0025A747
		[CompilerGenerated]
		private void <DoOpen>g__FalseWrapper|3_1()
		{
			base.SendEvent(false);
		}

		// Token: 0x06007FB5 RID: 32693 RVA: 0x0025C550 File Offset: 0x0025A750
		[CompilerGenerated]
		private void <DoOpen>g__TrueWrapper|3_2()
		{
			base.SendEvent(true);
		}

		// Token: 0x04007F39 RID: 32569
		[ObjectType(typeof(FullQuestBase))]
		public FsmObject Quest;

		// Token: 0x04007F3A RID: 32570
		public FsmBool ConsumeCurrency;
	}
}
