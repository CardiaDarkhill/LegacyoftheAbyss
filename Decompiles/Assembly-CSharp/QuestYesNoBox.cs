using System;
using System.Linq;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063B RID: 1595
public class QuestYesNoBox : YesNoBox
{
	// Token: 0x0600393F RID: 14655 RVA: 0x000FC04C File Offset: 0x000FA24C
	protected override void Awake()
	{
		base.Awake();
		if (!QuestYesNoBox._instance)
		{
			QuestYesNoBox._instance = this;
		}
		this.pane.OnPaneStart += base.OnAppearing;
		this.pane.PaneOpenedAnimEnd += base.OnAppeared;
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x000FC09F File Offset: 0x000FA29F
	private void OnDestroy()
	{
		if (QuestYesNoBox._instance == this)
		{
			QuestYesNoBox._instance = null;
		}
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x000FC0B4 File Offset: 0x000FA2B4
	private void UpdateHeight()
	{
		if (this.questNameText && this.layoutElement)
		{
			string text = this.questNameText.text;
			TMP_TextInfo textInfo = this.questNameText.GetTextInfo(text);
			float minHeight = this.baseHeight + this.heightPerLine * (float)textInfo.lineCount;
			this.layoutElement.minHeight = minHeight;
		}
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x000FC118 File Offset: 0x000FA318
	public static void Open(Action yes, Action no, bool returnHud, FullQuestBase quest, bool beginQuest)
	{
		QuestYesNoBox.<>c__DisplayClass12_0 CS$<>8__locals1 = new QuestYesNoBox.<>c__DisplayClass12_0();
		CS$<>8__locals1.quest = quest;
		CS$<>8__locals1.yes = yes;
		if (!QuestYesNoBox._instance || !CS$<>8__locals1.quest)
		{
			return;
		}
		QuestYesNoBox._instance.questDisplay.SetQuest(CS$<>8__locals1.quest, false);
		QuestYesNoBox._instance.UpdateHeight();
		if (CS$<>8__locals1.quest.Targets.Count == 1)
		{
			DeliveryQuestItem deliveryQuestItem = (from target in CS$<>8__locals1.quest.Targets
			select target.Counter).OfType<DeliveryQuestItem>().FirstOrDefault<DeliveryQuestItem>();
			if (deliveryQuestItem)
			{
				QuestYesNoBox._instance.rewardAmountText.text = CS$<>8__locals1.quest.RewardCount.ToString();
				QuestYesNoBox._instance.deliverIcon.sprite = deliveryQuestItem.GetIcon(CollectableItem.ReadSource.GetPopup);
				QuestYesNoBox._instance.deliverGroup.SetActive(true);
			}
			else
			{
				QuestYesNoBox._instance.deliverGroup.SetActive(false);
			}
		}
		else
		{
			QuestYesNoBox._instance.deliverGroup.SetActive(false);
		}
		QuestYesNoBox._instance.InternalOpen(beginQuest ? new Action(CS$<>8__locals1.<Open>g__NewYes|0) : CS$<>8__locals1.yes, no, returnHud);
	}

	// Token: 0x06003943 RID: 14659 RVA: 0x000FC25B File Offset: 0x000FA45B
	public static void ForceClose()
	{
		if (QuestYesNoBox._instance)
		{
			QuestYesNoBox._instance.DoEnd();
		}
	}

	// Token: 0x04003C0A RID: 15370
	[Space]
	[SerializeField]
	private InventoryItemQuest questDisplay;

	// Token: 0x04003C0B RID: 15371
	[SerializeField]
	private GameObject deliverGroup;

	// Token: 0x04003C0C RID: 15372
	[SerializeField]
	private SpriteRenderer deliverIcon;

	// Token: 0x04003C0D RID: 15373
	[SerializeField]
	private TMP_Text rewardAmountText;

	// Token: 0x04003C0E RID: 15374
	[Header("Dynamic Height")]
	[SerializeField]
	private TextMeshPro questNameText;

	// Token: 0x04003C0F RID: 15375
	[SerializeField]
	private LayoutElement layoutElement;

	// Token: 0x04003C10 RID: 15376
	[SerializeField]
	private float baseHeight = 2.3f;

	// Token: 0x04003C11 RID: 15377
	[SerializeField]
	private float heightPerLine = 0.6f;

	// Token: 0x04003C12 RID: 15378
	private static QuestYesNoBox _instance;
}
