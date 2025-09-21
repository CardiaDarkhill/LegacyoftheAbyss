using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000593 RID: 1427
public class InventoryItemQuestManager : QuestItemManager, IInventoryPaneAvailabilityProvider
{
	// Token: 0x06003346 RID: 13126 RVA: 0x000E3F2C File Offset: 0x000E212C
	private void Start()
	{
		this.inputHandler = ManagerSingleton<InputHandler>.Instance;
		InventoryPane component = base.GetComponent<InventoryPane>();
		component.OnPaneStart += delegate()
		{
			this.isPaneVisible = true;
		};
		component.OnPaneEnd += delegate()
		{
			this.isPaneVisible = false;
		};
		this.isPaneVisible = component.IsPaneActive;
		this.SetToggleCompletedText(this.toggleCompletedOnText);
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x000E3F8C File Offset: 0x000E218C
	private void Update()
	{
		if (!this.isPaneVisible || base.IsActionsBlocked)
		{
			return;
		}
		if (Platform.Current.GetMenuAction(this.inputHandler.inputActions, false, false) == Platform.MenuActions.Super && this.completedQuests.Count > 0)
		{
			this.isCompletedQuestsVisible = !this.isCompletedQuestsVisible;
			base.UpdateList();
			this.SetToggleCompletedText(this.isCompletedQuestsVisible ? this.toggleCompletedOffText : this.toggleCompletedOnText);
			this.toggleAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		}
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x000E4025 File Offset: 0x000E2225
	private void SetToggleCompletedText(string text)
	{
		if (this.toggleCompletedText)
		{
			this.toggleCompletedText.text = text;
		}
		if (this.toggleCompletedTextLayout)
		{
			this.toggleCompletedTextLayout.SetLayoutAll();
		}
	}

	// Token: 0x06003349 RID: 13129 RVA: 0x000E4058 File Offset: 0x000E2258
	public bool IsAvailable()
	{
		return QuestManager.IsAnyQuestVisible();
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x000E405F File Offset: 0x000E225F
	private static IEnumerable<BasicQuestBase> EnumerateSubQuests(BasicQuestBase quest)
	{
		yield return quest;
		MainQuest mainQuest = quest as MainQuest;
		if (mainQuest == null)
		{
			yield break;
		}
		bool mainQuestComplete = mainQuest.IsCompleted;
		foreach (SubQuest subQuest in mainQuest.SubQuests)
		{
			SubQuest current = subQuest.GetCurrent();
			if (!mainQuestComplete || current.IsLinkedBoolComplete)
			{
				yield return current;
			}
		}
		IEnumerator<SubQuest> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x000E4070 File Offset: 0x000E2270
	protected override List<BasicQuestBase> GetItems()
	{
		return (from quest in (from quest in QuestManager.GetAcceptedQuests()
		where !this.IsInMainQuestSection(quest)
		select quest).SelectMany(new Func<BasicQuestBase, IEnumerable<BasicQuestBase>>(InventoryItemQuestManager.EnumerateSubQuests))
		where !quest.IsHidden
		select quest).ToList<BasicQuestBase>();
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x000E40D0 File Offset: 0x000E22D0
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemQuest> questItems, List<BasicQuestBase> quests)
	{
		this.currentQuests.Clear();
		this.completedQuests.Clear();
		this.rumours.Clear();
		foreach (BasicQuestBase basicQuestBase in quests)
		{
			IQuestWithCompletion questWithCompletion = basicQuestBase as IQuestWithCompletion;
			if (questWithCompletion != null)
			{
				if (questWithCompletion.IsCompleted)
				{
					this.completedQuests.Add(basicQuestBase);
				}
				else
				{
					this.currentQuests.Add(basicQuestBase);
				}
			}
			else
			{
				this.rumours.Add(basicQuestBase);
			}
		}
		this.currentQuests.AddRange(this.rumours);
		List<InventoryItemQuest> range = questItems.GetRange(0, this.currentQuests.Count);
		for (int i = 0; i < range.Count; i++)
		{
			BasicQuestBase basicQuestBase2 = this.currentQuests[i];
			InventoryItemQuest inventoryItemQuest = range[i];
			inventoryItemQuest.gameObject.SetActive(true);
			inventoryItemQuest.SetQuest(basicQuestBase2, false);
			inventoryItemQuest.gameObject.name = "Quest (" + basicQuestBase2.name + ")";
		}
		List<InventoryItemQuest> range2 = questItems.GetRange(this.currentQuests.Count, this.completedQuests.Count);
		if (this.isCompletedQuestsVisible)
		{
			for (int j = 0; j < range2.Count; j++)
			{
				BasicQuestBase basicQuestBase3 = this.completedQuests[j];
				InventoryItemQuest inventoryItemQuest2 = range2[j];
				inventoryItemQuest2.gameObject.SetActive(true);
				inventoryItemQuest2.SetQuest(basicQuestBase3, true);
				inventoryItemQuest2.gameObject.name = "Completed Quest (" + basicQuestBase3.name + ")";
			}
		}
		else
		{
			bool flag = false;
			foreach (InventoryItemQuest inventoryItemQuest3 in range2)
			{
				inventoryItemQuest3.gameObject.SetActive(false);
				if (!flag && base.CurrentSelected == inventoryItemQuest3)
				{
					flag = true;
				}
			}
			if (flag)
			{
				bool flag2 = false;
				if (range.Count > 0)
				{
					for (int k = 1; k <= range.Count; k++)
					{
						List<InventoryItemQuest> list = range;
						int num = k;
						InventoryItemQuest inventoryItemQuest4 = list[list.Count - num];
						if (inventoryItemQuest4.gameObject.activeSelf)
						{
							base.SetSelected(inventoryItemQuest4, null, false);
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					base.SetSelected(InventoryItemManager.SelectedActionType.Default, false);
				}
			}
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Header = this.currentHeading,
				Items = range.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			},
			new InventoryItemGrid.GridSection
			{
				Header = this.completedHeading,
				Items = range2.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			}
		};
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x000E43A8 File Offset: 0x000E25A8
	protected override InventoryItemSelectable GetStartSelectable()
	{
		if (!QuestManager.UpdatedQuest)
		{
			return base.GetStartSelectable();
		}
		InventoryItemQuest inventoryItemQuest = base.GetSelectables(null).FirstOrDefault((InventoryItemQuest quest) => QuestManager.UpdatedQuest == quest.Quest);
		QuestManager.UpdatedQuest = null;
		if (inventoryItemQuest && inventoryItemQuest.gameObject.activeSelf)
		{
			return inventoryItemQuest;
		}
		return base.GetStartSelectable();
	}

	// Token: 0x0600334E RID: 13134 RVA: 0x000E4418 File Offset: 0x000E2618
	private bool IsInMainQuestSection(BasicQuestBase quest)
	{
		MainQuest mainQuest = quest as MainQuest;
		return !(mainQuest == null) && !mainQuest.IsCompleted;
	}

	// Token: 0x0600334F RID: 13135 RVA: 0x000E4440 File Offset: 0x000E2640
	protected override void OnItemListSetup()
	{
		List<BasicQuestBase> list = (from quest in QuestManager.GetAcceptedQuests().Where(new Func<BasicQuestBase, bool>(this.IsInMainQuestSection))
		where !quest.IsHidden
		select quest).ToList<BasicQuestBase>();
		this.mainQuestTemplateItem.gameObject.SetActive(false);
		if (this.spawnedMainQuestItems == null)
		{
			this.spawnedMainQuestItems = new List<InventoryItemMainQuest>(list.Count);
		}
		for (int i = list.Count - this.spawnedMainQuestItems.Count; i > 0; i--)
		{
			InventoryItemMainQuest item = Object.Instantiate<InventoryItemMainQuest>(this.mainQuestTemplateItem, this.mainQuestTemplateItem.transform.parent);
			this.spawnedMainQuestItems.Add(item);
			item.OnSelected += delegate(InventoryItemSelectable selectable)
			{
				base.ItemList.ScrollTo(selectable, false);
			};
			item.OnSubSelected += delegate(InventoryItemSelectable _)
			{
				this.ItemList.ScrollTo(item, false);
			};
		}
		for (int j = 0; j < list.Count; j++)
		{
			BasicQuestBase basicQuestBase = list[j];
			InventoryItemMainQuest inventoryItemMainQuest = this.spawnedMainQuestItems[j];
			inventoryItemMainQuest.gameObject.SetActive(true);
			inventoryItemMainQuest.SetQuest(basicQuestBase, false);
			inventoryItemMainQuest.gameObject.name = "Main Quest (" + basicQuestBase.name + ")";
			if (j < list.Count - 1)
			{
				InventoryItemMainQuest inventoryItemMainQuest2 = this.spawnedMainQuestItems[j + 1];
				inventoryItemMainQuest.Selectables[1] = inventoryItemMainQuest2;
				inventoryItemMainQuest2.Selectables[0] = inventoryItemMainQuest;
			}
			else
			{
				InventoryItemGrid.LinkVertical(inventoryItemMainQuest, base.ItemList);
				inventoryItemMainQuest.Selectables[1] = base.ItemList.GetFirst();
			}
		}
		for (int k = list.Count; k < this.spawnedMainQuestItems.Count; k++)
		{
			this.spawnedMainQuestItems[k].gameObject.SetActive(false);
		}
		if (list.Count <= 0)
		{
			this.currentHeading.gameObject.SetActive(false);
		}
		this.itemListLayout.ForceUpdateLayoutNoCanvas();
		this.itemListScrollView.FullUpdate();
		this.toggleCompletedParent.SetActive(this.completedQuests.Count > 0);
	}

	// Token: 0x06003350 RID: 13136 RVA: 0x000E467C File Offset: 0x000E287C
	protected override string FormatDisplayName(string displayName)
	{
		return displayName.Replace("\n", "");
	}

	// Token: 0x04003715 RID: 14101
	[Header("Quest Inventory")]
	[SerializeField]
	private Transform currentHeading;

	// Token: 0x04003716 RID: 14102
	[SerializeField]
	private Transform completedHeading;

	// Token: 0x04003717 RID: 14103
	[Space]
	[SerializeField]
	private LayoutGroup itemListLayout;

	// Token: 0x04003718 RID: 14104
	[SerializeField]
	private ScrollView itemListScrollView;

	// Token: 0x04003719 RID: 14105
	[SerializeField]
	private InventoryItemMainQuest mainQuestTemplateItem;

	// Token: 0x0400371A RID: 14106
	[Space]
	[SerializeField]
	private GameObject toggleCompletedParent;

	// Token: 0x0400371B RID: 14107
	[SerializeField]
	private TMP_Text toggleCompletedText;

	// Token: 0x0400371C RID: 14108
	[SerializeField]
	private TextMeshProContainerFitter toggleCompletedTextLayout;

	// Token: 0x0400371D RID: 14109
	[SerializeField]
	private LocalisedString toggleCompletedOnText;

	// Token: 0x0400371E RID: 14110
	[SerializeField]
	private LocalisedString toggleCompletedOffText;

	// Token: 0x0400371F RID: 14111
	[SerializeField]
	private AudioEvent toggleAudio;

	// Token: 0x04003720 RID: 14112
	private readonly List<BasicQuestBase> currentQuests = new List<BasicQuestBase>();

	// Token: 0x04003721 RID: 14113
	private readonly List<BasicQuestBase> completedQuests = new List<BasicQuestBase>();

	// Token: 0x04003722 RID: 14114
	private readonly List<BasicQuestBase> rumours = new List<BasicQuestBase>();

	// Token: 0x04003723 RID: 14115
	private bool isPaneVisible;

	// Token: 0x04003724 RID: 14116
	private bool isCompletedQuestsVisible;

	// Token: 0x04003725 RID: 14117
	private List<InventoryItemMainQuest> spawnedMainQuestItems;

	// Token: 0x04003726 RID: 14118
	private InputHandler inputHandler;
}
