using System;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x020005A1 RID: 1441
public abstract class QuestItemManager : InventoryItemListManager<InventoryItemQuest, BasicQuestBase>
{
	// Token: 0x060033C4 RID: 13252 RVA: 0x000E7015 File Offset: 0x000E5215
	protected override void Awake()
	{
		base.Awake();
		this.fontSize = this.descriptionText.fontSize;
		this.paraSpacing = this.descriptionText.paragraphSpacing;
		this.ResetExtraDisplay();
	}

	// Token: 0x060033C5 RID: 13253 RVA: 0x000E7045 File Offset: 0x000E5245
	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		this.ResetExtraDisplay();
	}

	// Token: 0x060033C6 RID: 13254 RVA: 0x000E7054 File Offset: 0x000E5254
	private void ResetExtraDisplay()
	{
		if (this.descriptionGroup)
		{
			this.descriptionGroup.AlphaSelf = 0f;
		}
		if (this.questItemDescription)
		{
			this.questItemDescription.SetDisplay(null);
		}
		this.descriptionText.fontSize = this.fontSize;
		this.descriptionText.paragraphSpacing = this.paraSpacing;
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x000E70BC File Offset: 0x000E52BC
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		InventoryItemQuest inventoryItemQuest = selectable as InventoryItemQuest;
		if (inventoryItemQuest)
		{
			BasicQuestBase quest = inventoryItemQuest.Quest;
			if (!quest)
			{
				return;
			}
			FullQuestBase fullQuestBase = quest as FullQuestBase;
			if (fullQuestBase != null)
			{
				if (fullQuestBase.OverrideFontSize.IsEnabled)
				{
					this.descriptionText.fontSize = fullQuestBase.OverrideFontSize.Value;
				}
				if (fullQuestBase.OverrideParagraphSpacing.IsEnabled)
				{
					this.descriptionText.paragraphSpacing = fullQuestBase.OverrideParagraphSpacing.Value;
				}
			}
			if (this.descriptionGroup)
			{
				this.descriptionGroup.AlphaSelf = 1f;
			}
			if (this.typeText)
			{
				if (quest.QuestType)
				{
					this.typeText.text = quest.QuestType.DisplayName;
					this.typeText.color = quest.QuestType.TextColor;
				}
				else
				{
					this.typeText.text = "NO TYPE ASSIGNED";
					this.typeText.color = Color.magenta;
				}
			}
			if (this.questItemDescription)
			{
				this.questItemDescription.SetDisplay(quest);
			}
			if (this.locationText)
			{
				string location = quest.Location;
				this.locationText.text = location;
				this.locationText.gameObject.SetActive(!string.IsNullOrWhiteSpace(location));
				return;
			}
		}
		else if (this.descriptionGroup)
		{
			this.descriptionGroup.AlphaSelf = 1f;
		}
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x000E7237 File Offset: 0x000E5437
	public override bool MoveSelectionPage(InventoryItemManager.SelectionDirection direction)
	{
		return false;
	}

	// Token: 0x04003789 RID: 14217
	[SerializeField]
	private NestedFadeGroup descriptionGroup;

	// Token: 0x0400378A RID: 14218
	[SerializeField]
	private TextMeshPro typeText;

	// Token: 0x0400378B RID: 14219
	[SerializeField]
	private TextMeshPro locationText;

	// Token: 0x0400378C RID: 14220
	[SerializeField]
	private QuestItemDescription questItemDescription;

	// Token: 0x0400378D RID: 14221
	private float fontSize;

	// Token: 0x0400378E RID: 14222
	private float paraSpacing;
}
