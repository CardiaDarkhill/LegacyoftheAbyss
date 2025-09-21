using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class CollectionViewBoard : InventoryItemListManager<CollectionViewBoardItem, ICollectionViewerItem>
{
	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06001215 RID: 4629 RVA: 0x000543D0 File Offset: 0x000525D0
	// (remove) Token: 0x06001216 RID: 4630 RVA: 0x00054408 File Offset: 0x00052608
	public event Action BoardClosed;

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001217 RID: 4631 RVA: 0x00054440 File Offset: 0x00052640
	protected override IEnumerable<InventoryItemSelectable> DefaultSelectables
	{
		get
		{
			if (this.newItems.Count <= 0)
			{
				return base.DefaultSelectables;
			}
			return this.newItems;
		}
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0005446C File Offset: 0x0005266C
	protected override void Awake()
	{
		base.Awake();
		this.headingTemplate.gameObject.SetActive(false);
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
				if (this.newItems.Count <= 0)
				{
					return;
				}
				float num = float.MaxValue;
				CollectionViewBoardItem item = null;
				foreach (CollectionViewBoardItem collectionViewBoardItem in this.newItems)
				{
					float y = collectionViewBoardItem.transform.localPosition.y;
					if (y <= num)
					{
						num = y;
						item = collectionViewBoardItem;
					}
				}
				base.ItemList.ScrollTo(item, true);
			};
			this.pane.PaneOpenedAnimEnd += delegate()
			{
				this.isPaneOpenComplete = true;
				if (this.newItemsAppearRoutine != null)
				{
					return;
				}
				base.IsActionsBlocked = false;
				this.input.enabled = true;
				if (this.cursor)
				{
					this.cursor.Activate();
				}
				base.SetSelected(InventoryItemManager.SelectedActionType.Default, false);
			};
			this.pane.PaneClosedAnimEnd += delegate()
			{
				Action boardClosed = this.BoardClosed;
				if (boardClosed != null)
				{
					boardClosed();
				}
				this.input.enabled = false;
				GameCameras.instance.HUDIn();
				this.isPaneOpenComplete = false;
			};
		}
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x00054512 File Offset: 0x00052712
	private void OnDisable()
	{
		this.displayingNewItems = false;
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x0005451C File Offset: 0x0005271C
	protected override List<ICollectionViewerItem> GetItems()
	{
		this.currentItemList.Clear();
		if (!this.owner)
		{
			return this.currentItemList;
		}
		bool constructedFarsight = PlayerData.instance.ConstructedFarsight;
		using (IEnumerator<CollectionViewerDesk.Section> enumerator = this.owner.Sections.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CollectionViewerDesk.Section section = enumerator.Current;
				if (section.IsActive)
				{
					section.CheckIsListActive(!constructedFarsight, delegate(ICollectionViewerItem item)
					{
						this.currentItemList.Add(item);
						this.reverseSectionMap[item] = section;
					});
				}
			}
		}
		return this.currentItemList;
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x000545D4 File Offset: 0x000527D4
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<CollectionViewBoardItem> selectableItems, List<ICollectionViewerItem> items)
	{
		this.cachedSelectableItems = selectableItems;
		if (!this.owner)
		{
			return new List<InventoryItemGrid.GridSection>();
		}
		this.newItems.Clear();
		CollectableRelic playingRelic = this.owner.GetPlayingRelic();
		PlayerData instance = PlayerData.instance;
		for (int i = 0; i < selectableItems.Count; i++)
		{
			CollectionViewBoardItem collectionViewBoardItem = selectableItems[i];
			ICollectionViewerItem collectionViewerItem = items[i];
			if (playingRelic && collectionViewerItem.name == playingRelic.name)
			{
				this.playingRelicItem = collectionViewBoardItem;
			}
			collectionViewBoardItem.gameObject.SetActive(true);
			collectionViewBoardItem.Board = this;
			bool flag;
			collectionViewBoardItem.SetItem(collectionViewerItem, out flag);
			if (flag)
			{
				CollectableItem itemByName = CollectableItemManager.GetItemByName(collectionViewBoardItem.name);
				if ((itemByName == null || ((ICollectionViewerItem)itemByName).CanDeposit) && collectionViewBoardItem.Item as CollectableItemMemento && instance.Collectables.GetData(collectionViewBoardItem.name).Amount > 0)
				{
					this.newItems.Add(collectionViewBoardItem);
				}
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
		return (from selectable in selectableItems
		group selectable by this.reverseSectionMap[selectable.Item] into @group
		select new InventoryItemGrid.GridSection
		{
			Items = @group.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>(),
			Header = this.GetHeadingForSection(@group.Key)
		}).ToList<InventoryItemGrid.GridSection>();
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x00054794 File Offset: 0x00052994
	private Transform GetHeadingForSection(CollectionViewerDesk.Section section)
	{
		Transform transform;
		if (!this.headingMap.TryGetValue(section, out transform))
		{
			TMP_Text tmp_Text = Object.Instantiate<TMP_Text>(this.headingTemplate, this.headingTemplate.transform.parent);
			tmp_Text.text = section.Heading;
			tmp_Text.gameObject.SetActive(true);
			transform = tmp_Text.transform;
			this.headingMap[section] = transform;
		}
		return transform;
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x00054800 File Offset: 0x00052A00
	public void OpenBoard(CollectionViewerDesk setOwner)
	{
		base.IsActionsBlocked = true;
		this.owner = setOwner;
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

	// Token: 0x0600121E RID: 4638 RVA: 0x0005485E File Offset: 0x00052A5E
	private void CloseBoard()
	{
		base.IsActionsBlocked = true;
		if (this.cursor)
		{
			this.cursor.Deactivate();
		}
		if (this.pane)
		{
			this.pane.PaneEnd();
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x00054897 File Offset: 0x00052A97
	protected override void OnItemInstantiated(CollectionViewBoardItem item)
	{
		item.Canceled += this.CloseBoard;
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x000548AC File Offset: 0x00052AAC
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

	// Token: 0x06001221 RID: 4641 RVA: 0x0005490C File Offset: 0x00052B0C
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		if (this.displayingNewItems && this.newItems.Contains(selectable))
		{
			return;
		}
		CollectionViewBoardItem collectionViewBoardItem = selectable as CollectionViewBoardItem;
		if (collectionViewBoardItem == null || collectionViewBoardItem.Item == null || !collectionViewBoardItem.Item.IsVisibleInCollection())
		{
			this.SetDisplay(selectable.gameObject);
			return;
		}
		base.SetDisplay(selectable);
		if (this.nextActionButton)
		{
			this.nextActionButton.SetActive(this.cachedSelectableItems.Count > 1 && collectionViewBoardItem != null && ((InventoryItemListManager<CollectionViewBoardItem, ICollectionViewerItem>.IMoveNextButton)collectionViewBoardItem).WillSubmitMoveNext);
		}
		if (!this.CanPlayRelic(collectionViewBoardItem))
		{
			return;
		}
		if (this.IsPlaying(collectionViewBoardItem))
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

	// Token: 0x06001222 RID: 4642 RVA: 0x000549E4 File Offset: 0x00052BE4
	public bool CanPlayRelic(CollectionViewBoardItem item)
	{
		CollectableRelic collectableRelic = item.Item as CollectableRelic;
		return !(collectableRelic == null) && collectableRelic.IsPlayable && this.owner && this.owner.HasGramaphone;
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x00054A2C File Offset: 0x00052C2C
	public bool TryPlayRelic(CollectionViewBoardItem item)
	{
		if (!this.owner || !this.owner.HasGramaphone)
		{
			return false;
		}
		if (this.playingRelicItem)
		{
			this.owner.StopPlayingRelic();
			if (this.playingRelicItem == item)
			{
				this.playingRelicItem = null;
				this.SetDisplay(item);
				return true;
			}
		}
		this.playingRelicItem = item;
		if (item == null)
		{
			return false;
		}
		CollectableRelic collectableRelic = item.Item as CollectableRelic;
		if (collectableRelic == null)
		{
			return false;
		}
		bool flag = false;
		if (collectableRelic.GramaphoneClip)
		{
			this.owner.PlayOnGramaphone(collectableRelic);
			flag = true;
		}
		if (flag)
		{
			this.SetDisplay(item);
		}
		return flag;
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x00054ADC File Offset: 0x00052CDC
	public bool IsPlaying(CollectionViewBoardItem relic)
	{
		return this.playingRelicItem && relic == this.playingRelicItem;
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x00054AFC File Offset: 0x00052CFC
	public void UpdateRelicItemsIsPlaying()
	{
		foreach (CollectionViewBoardItem collectionViewBoardItem in this.cachedSelectableItems)
		{
			collectionViewBoardItem.UpdatedIsPlaying();
		}
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x00054B4C File Offset: 0x00052D4C
	private IEnumerator NewItemsAppear()
	{
		base.IsActionsBlocked = true;
		this.BlockInput(true);
		yield return new WaitForSeconds(this.newItemAppearDelay);
		while (!this.isPaneOpenComplete)
		{
			yield return null;
		}
		PlayerData pd = PlayerData.instance;
		foreach (CollectionViewBoardItem collectionViewBoardItem in this.newItems)
		{
			pd.MementosDeposited.SetData(collectionViewBoardItem.name, new CollectableMementosData.Data
			{
				IsDeposited = true
			});
			pd.Collectables.SetData(collectionViewBoardItem.name, default(CollectableItemsData.Data));
			CollectableItemManager.IncrementVersion();
			this.owner.DoMementoDeposit(collectionViewBoardItem.name);
			collectionViewBoardItem.DoNewAppear();
			yield return new WaitForSeconds(this.newItemAppearOffset);
		}
		List<CollectionViewBoardItem>.Enumerator enumerator = default(List<CollectionViewBoardItem>.Enumerator);
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
		this.newItems.Clear();
		this.BlockInput(false);
		this.newItemsAppearRoutine = null;
		yield break;
		yield break;
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x00054B5C File Offset: 0x00052D5C
	private void BlockInput(bool state)
	{
		this.didBlockInput = state;
		InventoryPaneInput.IsInputBlocked = state;
	}

	// Token: 0x040010F1 RID: 4337
	[Space]
	[SerializeField]
	private GameObject playPrompt;

	// Token: 0x040010F2 RID: 4338
	[SerializeField]
	private GameObject stopPrompt;

	// Token: 0x040010F3 RID: 4339
	[Space]
	[SerializeField]
	private float newItemAppearDelay;

	// Token: 0x040010F4 RID: 4340
	[SerializeField]
	private float newItemAppearOffset;

	// Token: 0x040010F5 RID: 4341
	[Space]
	[SerializeField]
	private TMP_Text headingTemplate;

	// Token: 0x040010F6 RID: 4342
	[Space]
	[SerializeField]
	private GameObject nextActionButton;

	// Token: 0x040010F7 RID: 4343
	private bool isPaneOpenComplete;

	// Token: 0x040010F8 RID: 4344
	private Coroutine newItemsAppearRoutine;

	// Token: 0x040010F9 RID: 4345
	private readonly List<CollectionViewBoardItem> newItems = new List<CollectionViewBoardItem>();

	// Token: 0x040010FA RID: 4346
	private bool displayingNewItems;

	// Token: 0x040010FB RID: 4347
	private InventoryPaneInput input;

	// Token: 0x040010FC RID: 4348
	private InventoryPaneStandalone pane;

	// Token: 0x040010FD RID: 4349
	private CollectionViewerDesk owner;

	// Token: 0x040010FE RID: 4350
	private CollectionViewBoardItem playingRelicItem;

	// Token: 0x040010FF RID: 4351
	private List<CollectionViewBoardItem> cachedSelectableItems;

	// Token: 0x04001100 RID: 4352
	private readonly List<ICollectionViewerItem> currentItemList = new List<ICollectionViewerItem>();

	// Token: 0x04001101 RID: 4353
	private readonly Dictionary<ICollectionViewerItem, CollectionViewerDesk.Section> reverseSectionMap = new Dictionary<ICollectionViewerItem, CollectionViewerDesk.Section>();

	// Token: 0x04001102 RID: 4354
	private readonly Dictionary<CollectionViewerDesk.Section, Transform> headingMap = new Dictionary<CollectionViewerDesk.Section, Transform>();

	// Token: 0x04001103 RID: 4355
	private bool didBlockInput;
}
