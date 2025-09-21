using System;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001B4 RID: 436
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Quest Display)")]
public class CollectableItemQuestDisplay : CollectableItem
{
	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06001109 RID: 4361 RVA: 0x000506B7 File Offset: 0x0004E8B7
	public override bool DisplayAmount
	{
		get
		{
			return this.displayCollectedAmount;
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600110A RID: 4362 RVA: 0x000506C0 File Offset: 0x0004E8C0
	public override int CollectedAmount
	{
		get
		{
			if (!this.quest || !this.quest.IsAccepted || this.quest.IsCompleted)
			{
				return 0;
			}
			if (!this.displayCollectedAmount)
			{
				return 1;
			}
			return this.quest.Counters.FirstOrDefault<int>();
		}
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x00050710 File Offset: 0x0004E910
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		if ((readSource == CollectableItem.ReadSource.GetPopup || readSource == CollectableItem.ReadSource.TakePopup) && !this.pickupDisplayName.IsEmpty)
		{
			return this.pickupDisplayName;
		}
		return this.GetCurrentState().DisplayName;
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x00050743 File Offset: 0x0004E943
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		return this.GetCurrentState().Description;
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x00050755 File Offset: 0x0004E955
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		return this.GetCurrentState().Icon;
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x00050764 File Offset: 0x0004E964
	private CollectableItemQuestDisplay.ItemState GetCurrentState()
	{
		if (!this.quest)
		{
			return default(CollectableItemQuestDisplay.ItemState);
		}
		ValueTuple<FullQuestBase.QuestTarget, int> valueTuple = this.quest.TargetsAndCounters.FirstOrDefault<ValueTuple<FullQuestBase.QuestTarget, int>>();
		int num = valueTuple.Item2;
		int count = valueTuple.Item1.Count;
		int num2 = this.states.Length - 1;
		if (this.countMapping == CollectableItemQuestDisplay.CountMappings.Percentage)
		{
			float num3 = (float)num2 * ((float)num / (float)count);
			if (num3 > 0f && num3 < 1f)
			{
				num = 1;
			}
			else
			{
				num = Mathf.FloorToInt(num3);
			}
		}
		if (num > num2)
		{
			num = num2;
		}
		return this.states[num];
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x000507F8 File Offset: 0x0004E9F8
	public override void SetupExtraDescription(GameObject obj)
	{
		if (!this.quest)
		{
			return;
		}
		QuestItemDescription component = obj.GetComponent<QuestItemDescription>();
		if (!component)
		{
			return;
		}
		component.SetDisplay(this.quest);
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x00050830 File Offset: 0x0004EA30
	public override bool IsAtMax()
	{
		if (!this.quest)
		{
			return base.IsAtMax();
		}
		ValueTuple<FullQuestBase.QuestTarget, int> valueTuple = this.quest.TargetsAndCounters.FirstOrDefault<ValueTuple<FullQuestBase.QuestTarget, int>>();
		return valueTuple.Item2 >= valueTuple.Item1.Count;
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x00050878 File Offset: 0x0004EA78
	protected override void AddAmount(int amount)
	{
		if (!this.quest)
		{
			return;
		}
		for (int i = 0; i < amount; i++)
		{
			this.quest.IncrementQuestCounter();
		}
	}

	// Token: 0x04001020 RID: 4128
	[Space]
	[SerializeField]
	private Quest quest;

	// Token: 0x04001021 RID: 4129
	[SerializeField]
	private CollectableItemQuestDisplay.CountMappings countMapping;

	// Token: 0x04001022 RID: 4130
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString pickupDisplayName;

	// Token: 0x04001023 RID: 4131
	[SerializeField]
	private CollectableItemQuestDisplay.ItemState[] states;

	// Token: 0x04001024 RID: 4132
	[SerializeField]
	private bool displayCollectedAmount;

	// Token: 0x020014F4 RID: 5364
	private enum CountMappings
	{
		// Token: 0x04008555 RID: 34133
		Direct,
		// Token: 0x04008556 RID: 34134
		Percentage
	}

	// Token: 0x020014F5 RID: 5365
	[Serializable]
	private struct ItemState
	{
		// Token: 0x04008557 RID: 34135
		public LocalisedString DisplayName;

		// Token: 0x04008558 RID: 34136
		public LocalisedString Description;

		// Token: 0x04008559 RID: 34137
		public Sprite Icon;
	}
}
