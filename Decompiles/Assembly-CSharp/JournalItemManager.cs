using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class JournalItemManager : InventoryItemListManager<JournalEntryItem, EnemyJournalRecord>, IInventoryPaneAvailabilityProvider
{
	// Token: 0x06001B28 RID: 6952 RVA: 0x0007E33E File Offset: 0x0007C53E
	public bool IsAvailable()
	{
		return !CollectableItemManager.IsInHiddenMode() && EnemyJournalManager.GetKilledEnemies().Count > 0;
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x0007E356 File Offset: 0x0007C556
	protected override List<EnemyJournalRecord> GetItems()
	{
		if (!PlayerData.instance.ConstructedFarsight)
		{
			return EnemyJournalManager.GetKilledEnemies();
		}
		return EnemyJournalManager.GetRequiredEnemies();
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x0007E370 File Offset: 0x0007C570
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<JournalEntryItem> selectableItems, List<EnemyJournalRecord> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(true);
			selectableItems[i].Setup(items[i]);
		}
		if (Application.isPlaying)
		{
			if (PlayerData.instance.ConstructedFarsight)
			{
				this.completionParent.SetActive(true);
				int count = items.Count;
				int count2 = items.Count((EnemyJournalRecord record) => record.IsVisible);
				int count3 = items.Count((EnemyJournalRecord record) => record.KillCount >= record.KillsRequired);
				this.encounteredText.SetValues(count2, count);
				this.completedText.SetValues(count3, count);
			}
			else
			{
				this.completionParent.SetActive(false);
			}
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = selectableItems.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			}
		};
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x0007E474 File Offset: 0x0007C674
	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if (this.notesGroup)
		{
			this.notesGroup.AlphaSelf = 0f;
		}
		if (this.notesText)
		{
			this.notesText.text = string.Empty;
		}
		if (this.enemySprite)
		{
			this.enemySprite.sprite = null;
		}
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x0007E4DC File Offset: 0x0007C6DC
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		JournalEntryItem journalEntryItem = selectable as JournalEntryItem;
		if (journalEntryItem == null || !journalEntryItem.Record.IsVisible)
		{
			return;
		}
		int killCount = journalEntryItem.Record.KillCount;
		int killsRequired = journalEntryItem.Record.KillsRequired;
		bool flag = killCount >= killsRequired;
		if (this.notesText)
		{
			this.notesText.text = (flag ? journalEntryItem.Record.Notes : string.Format(this.notesLockedText, killsRequired - killCount));
		}
		if (this.notesGroup)
		{
			this.notesGroup.AlphaSelf = (flag ? 1f : UI.DisabledUiTextColor.r);
		}
		if (this.enemySprite)
		{
			this.enemySprite.sprite = journalEntryItem.Record.EnemySprite;
		}
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x0007E5C4 File Offset: 0x0007C7C4
	public override void InstantScroll()
	{
		EnemyJournalRecord updatedRecord = EnemyJournalManager.UpdatedRecord;
		if (updatedRecord == null)
		{
			base.InstantScroll();
			return;
		}
		InventoryItemSelectable startSelectable = this.GetStartSelectable();
		EnemyJournalManager.UpdatedRecord = updatedRecord;
		base.ItemList.ScrollTo(startSelectable, true);
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x0007E604 File Offset: 0x0007C804
	protected override InventoryItemSelectable GetStartSelectable()
	{
		if (!EnemyJournalManager.UpdatedRecord)
		{
			return base.GetStartSelectable();
		}
		JournalEntryItem journalEntryItem = base.GetSelectables(null).FirstOrDefault((JournalEntryItem entry) => entry.Record == EnemyJournalManager.UpdatedRecord);
		EnemyJournalManager.UpdatedRecord = null;
		if (!journalEntryItem)
		{
			return base.GetStartSelectable();
		}
		return journalEntryItem;
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x0007E666 File Offset: 0x0007C866
	public override bool MoveSelectionPage(InventoryItemManager.SelectionDirection direction)
	{
		return false;
	}

	// Token: 0x04001A1A RID: 6682
	[SerializeField]
	private NestedFadeGroupBase notesGroup;

	// Token: 0x04001A1B RID: 6683
	[SerializeField]
	private TextMeshPro notesText;

	// Token: 0x04001A1C RID: 6684
	[SerializeField]
	private LocalisedString notesLockedText;

	// Token: 0x04001A1D RID: 6685
	[SerializeField]
	private SpriteRenderer enemySprite;

	// Token: 0x04001A1E RID: 6686
	[SerializeField]
	private GameObject completionParent;

	// Token: 0x04001A1F RID: 6687
	[SerializeField]
	private JournalItemManager.CompletionText encounteredText;

	// Token: 0x04001A20 RID: 6688
	[SerializeField]
	private JournalItemManager.CompletionText completedText;

	// Token: 0x020015E3 RID: 5603
	[Serializable]
	private class CompletionText
	{
		// Token: 0x06008846 RID: 34886 RVA: 0x00279BC3 File Offset: 0x00277DC3
		public void SetValues(int count, int total)
		{
			this.SetFormattedText(this.countText, ref this.initialCountText, count);
			this.SetFormattedText(this.totalText, ref this.initialTotalText, total);
		}

		// Token: 0x06008847 RID: 34887 RVA: 0x00279BEB File Offset: 0x00277DEB
		private void SetFormattedText(TextMeshPro currentText, ref string initialText, int num)
		{
			if (currentText)
			{
				if (string.IsNullOrEmpty(initialText))
				{
					initialText = currentText.text;
				}
				currentText.text = string.Format(initialText, num);
			}
		}

		// Token: 0x040088FA RID: 35066
		[SerializeField]
		private TextMeshPro countText;

		// Token: 0x040088FB RID: 35067
		private string initialCountText;

		// Token: 0x040088FC RID: 35068
		[SerializeField]
		private TextMeshPro totalText;

		// Token: 0x040088FD RID: 35069
		private string initialTotalText;
	}
}
