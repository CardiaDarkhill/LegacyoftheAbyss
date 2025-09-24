using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class CollectableRelicBoard : InventoryItemListManager<CollectableRelicBoardItem, CollectableRelic>
{
	// Token: 0x14000038 RID: 56
	// (add) Token: 0x060012EA RID: 4842 RVA: 0x00057004 File Offset: 0x00055204
	// (remove) Token: 0x060012EB RID: 4843 RVA: 0x0005703C File Offset: 0x0005523C
	public event Action BoardClosed;

	// Token: 0x060012EC RID: 4844 RVA: 0x00057074 File Offset: 0x00055274
	protected override void Awake()
	{
		base.Awake();
		this.input = base.GetComponent<InventoryPaneInput>();
		this.pane = base.GetComponent<InventoryPaneStandalone>();
		if (this.pane)
		{
			this.input.enabled = false;
			this.pane.SkipInputEnable = true;
			this.pane.OnPaneStart += delegate()
			{
				this.input.enabled = false;
				base.IsActionsBlocked = true;
			};
			this.pane.PaneOpenedAnimEnd += delegate()
			{
				if (this.newItemsAppearRoutine == null)
				{
					this.input.enabled = true;
					if (this.cursor)
					{
						this.cursor.Activate();
					}
					base.IsActionsBlocked = false;
				}
				base.SetSelected(InventoryItemManager.SelectedActionType.Default, false);
			};
			this.pane.PaneClosedAnimEnd += delegate()
			{
				if (this.BoardClosed != null)
				{
					this.BoardClosed();
				}
				this.input.enabled = false;
				if (this.cursor)
				{
					this.cursor.Deactivate();
				}
				base.IsActionsBlocked = false;
			};
		}
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00057109 File Offset: 0x00055309
	private void OnDisable()
	{
		if (this.didBlockInput)
		{
			this.BlockInput(false);
		}
		this.displayingNewItems = false;
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00057124 File Offset: 0x00055324
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<CollectableRelicBoardItem> selectableItems, List<CollectableRelic> items)
	{
		this.cachedSelectableItems = selectableItems;
		if (this.newItemsAppearRoutine != null)
		{
			base.StopCoroutine(this.newItemsAppearRoutine);
			this.newItemsAppearRoutine = null;
		}
		this.newItems.Clear();
		for (int i = 0; i < selectableItems.Count; i++)
		{
			CollectableRelicBoardItem collectableRelicBoardItem = selectableItems[i];
			collectableRelicBoardItem.gameObject.SetActive(true);
			collectableRelicBoardItem.Board = this;
			bool flag;
			collectableRelicBoardItem.SetRelic(items[i], out flag);
			if (flag)
			{
				this.newItems.Add(collectableRelicBoardItem);
			}
		}
		if (this.newItems.Count > 0)
		{
			this.displayingNewItems = true;
			this.nameText.text = "";
			this.descriptionText.text = "";
			if (this.playPrompt)
			{
				this.playPrompt.SetActive(false);
			}
			if (this.stopPrompt)
			{
				this.stopPrompt.SetActive(false);
			}
			if (this.nextActionButton)
			{
				this.nextActionButton.SetActive(false);
			}
			this.newItemsAppearRoutine = base.StartCoroutine(this.NewItemsAppear());
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = selectableItems.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			}
		};
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00057260 File Offset: 0x00055460
	protected override List<CollectableRelic> GetItems()
	{
		if (this.owner == null)
		{
			return new List<CollectableRelic>();
		}
		if (PlayerData.instance.ConstructedFarsight)
		{
			return this.owner.GetRelics().ToList<CollectableRelic>();
		}
		return this.owner.GetRelics().Where(delegate(CollectableRelic relic)
		{
			CollectableRelicsData.Data savedData = relic.SavedData;
			return savedData.IsCollected || savedData.IsDeposited;
		}).ToList<CollectableRelic>();
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x000572D4 File Offset: 0x000554D4
	public void OpenBoard(RelicBoardOwner setOwner)
	{
		this.owner = setOwner;
		this.playingRelic = this.owner.GetPlayingRelic();
		if (this.cursor)
		{
			this.cursor.Deactivate();
		}
		if (this.pane)
		{
			this.pane.PaneStart();
		}
		base.SetSelected(InventoryItemManager.SelectedActionType.Default, true);
		GameCameras.instance.HUDOut();
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x0005733C File Offset: 0x0005553C
	public void CloseBoard()
	{
		if (this.pane)
		{
			this.pane.PaneEnd();
		}
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00057356 File Offset: 0x00055556
	protected override void OnItemInstantiated(CollectableRelicBoardItem item)
	{
		item.Canceled += this.CloseBoard;
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x0005736C File Offset: 0x0005556C
	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if (this.playPrompt)
		{
			this.playPrompt.SetActive(false);
		}
		if (this.stopPrompt)
		{
			this.stopPrompt.SetActive(false);
		}
		if (this.nextActionButton)
		{
			this.nextActionButton.SetActive(true);
		}
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x000573CC File Offset: 0x000555CC
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		if (this.displayingNewItems && this.newItems.Contains(selectable))
		{
			return;
		}
		base.SetDisplay(selectable);
		CollectableRelicBoardItem collectableRelicBoardItem = selectable as CollectableRelicBoardItem;
		if (collectableRelicBoardItem == null)
		{
			return;
		}
		CollectableRelic relic = collectableRelicBoardItem.Relic;
		if (!relic)
		{
			return;
		}
		if (this.nextActionButton)
		{
			this.nextActionButton.SetActive(this.cachedSelectableItems.Count > 1);
		}
		if (!relic.SavedData.IsDeposited)
		{
			return;
		}
		if (!relic.IsPlayable)
		{
			return;
		}
		if (this.IsPlaying(collectableRelicBoardItem))
		{
			if (this.stopPrompt)
			{
				this.stopPrompt.SetActive(true);
				return;
			}
		}
		else if (this.playPrompt)
		{
			this.playPrompt.SetActive(true);
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00057494 File Offset: 0x00055694
	public bool TryPlayRelic(CollectableRelicBoardItem relic)
	{
		if (!this.owner || !this.owner.HasGramaphone)
		{
			return false;
		}
		if (this.playingRelic)
		{
			this.owner.StopPlayingRelic();
			if (this.playingRelic == relic.Relic)
			{
				this.playingRelic = null;
				this.SetDisplay(relic);
				return true;
			}
		}
		this.playingRelic = relic.Relic;
		if (!relic)
		{
			return false;
		}
		bool flag = false;
		if (relic.Relic.GramaphoneClip)
		{
			this.owner.PlayOnGramaphone(relic.Relic);
			flag = true;
		}
		if (flag)
		{
			this.SetDisplay(relic);
		}
		return flag;
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00057540 File Offset: 0x00055740
	public bool IsPlaying(CollectableRelicBoardItem relic)
	{
		return this.playingRelic && relic.Relic == this.playingRelic;
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x00057564 File Offset: 0x00055764
	public void UpdateRelicItemsIsPlaying()
	{
		foreach (CollectableRelicBoardItem collectableRelicBoardItem in this.cachedSelectableItems)
		{
			collectableRelicBoardItem.UpdatedIsPlaying();
		}
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x000575B4 File Offset: 0x000557B4
	private IEnumerator NewItemsAppear()
	{
		base.IsActionsBlocked = true;
		if (this.stopInputOnNewItems)
		{
			this.BlockInput(true);
		}
		yield return new WaitForSeconds(this.newItemAppearDelay);
		foreach (CollectableRelicBoardItem collectableRelicBoardItem in this.newItems)
		{
			collectableRelicBoardItem.DoNewAppear();
			CollectableRelic relic = collectableRelicBoardItem.Relic;
			if (relic)
			{
				CollectableItemRelicType relicType = relic.RelicType;
				if (relicType && relicType.RewardAmount > 0)
				{
					CurrencyManager.AddCurrency(relicType.RewardAmount, CurrencyType.Money, true);
				}
			}
			yield return new WaitForSeconds(this.newItemAppearOffset);
		}
		List<CollectableRelicBoardItem>.Enumerator enumerator = default(List<CollectableRelicBoardItem>.Enumerator);
		this.displayingNewItems = false;
		base.IsActionsBlocked = false;
		if (this.cursor)
		{
			this.cursor.Activate();
		}
		this.input.enabled = true;
		if (base.CurrentSelected)
		{
			base.SetSelected(base.CurrentSelected, null, false);
		}
		if (this.stopInputOnNewItems)
		{
			this.BlockInput(false);
		}
		this.newItemsAppearRoutine = null;
		yield break;
		yield break;
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x000575C4 File Offset: 0x000557C4
	private void BlockInput(bool state)
	{
		this.didBlockInput = state;
		InventoryPaneInput.IsInputBlocked = state;
	}

	// Token: 0x04001189 RID: 4489
	[Space]
	[SerializeField]
	private GameObject playPrompt;

	// Token: 0x0400118A RID: 4490
	[SerializeField]
	private GameObject stopPrompt;

	// Token: 0x0400118B RID: 4491
	[Space]
	[SerializeField]
	private float newItemAppearDelay;

	// Token: 0x0400118C RID: 4492
	[SerializeField]
	private float newItemAppearOffset;

	// Token: 0x0400118D RID: 4493
	[Space]
	[SerializeField]
	private GameObject nextActionButton;

	// Token: 0x0400118E RID: 4494
	[SerializeField]
	private bool stopInputOnNewItems;

	// Token: 0x0400118F RID: 4495
	private Coroutine newItemsAppearRoutine;

	// Token: 0x04001190 RID: 4496
	private bool displayingNewItems;

	// Token: 0x04001191 RID: 4497
	private InventoryPaneInput input;

	// Token: 0x04001192 RID: 4498
	private InventoryPaneStandalone pane;

	// Token: 0x04001193 RID: 4499
	private RelicBoardOwner owner;

	// Token: 0x04001194 RID: 4500
	private CollectableRelic playingRelic;

	// Token: 0x04001195 RID: 4501
	private List<CollectableRelicBoardItem> cachedSelectableItems;

	// Token: 0x04001196 RID: 4502
	private readonly List<CollectableRelicBoardItem> newItems = new List<CollectableRelicBoardItem>();

	// Token: 0x04001197 RID: 4503
	private bool didBlockInput;
}
