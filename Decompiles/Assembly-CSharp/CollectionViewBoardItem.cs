using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class CollectionViewBoardItem : InventoryItemUpdateable, InventoryItemListManager<CollectionViewBoardItem, ICollectionViewerItem>.IMoveNextButton
{
	// Token: 0x14000037 RID: 55
	// (add) Token: 0x0600122E RID: 4654 RVA: 0x00054D0C File Offset: 0x00052F0C
	// (remove) Token: 0x0600122F RID: 4655 RVA: 0x00054D44 File Offset: 0x00052F44
	public event Action Canceled;

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001230 RID: 4656 RVA: 0x00054D79 File Offset: 0x00052F79
	// (set) Token: 0x06001231 RID: 4657 RVA: 0x00054D81 File Offset: 0x00052F81
	public ICollectionViewerItem Item { get; private set; }

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001232 RID: 4658 RVA: 0x00054D8A File Offset: 0x00052F8A
	public override string DisplayName
	{
		get
		{
			if (this.Item == null)
			{
				return string.Empty;
			}
			return this.Item.GetCollectionName();
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001233 RID: 4659 RVA: 0x00054DA5 File Offset: 0x00052FA5
	public override string Description
	{
		get
		{
			if (this.Item == null)
			{
				return string.Empty;
			}
			return this.Item.GetCollectionDesc();
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001234 RID: 4660 RVA: 0x00054DC0 File Offset: 0x00052FC0
	// (set) Token: 0x06001235 RID: 4661 RVA: 0x00054DC8 File Offset: 0x00052FC8
	public CollectionViewBoard Board { get; set; }

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06001236 RID: 4662 RVA: 0x00054DD1 File Offset: 0x00052FD1
	public bool WillSubmitMoveNext
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001237 RID: 4663 RVA: 0x00054DD4 File Offset: 0x00052FD4
	// (set) Token: 0x06001238 RID: 4664 RVA: 0x00054E0C File Offset: 0x0005300C
	protected override bool IsSeen
	{
		get
		{
			ICollectionViewerItem item = this.Item;
			if (item == null || !item.IsVisibleInCollection())
			{
				return true;
			}
			if (!item.IsSeenOverridden)
			{
				return item.IsSeen;
			}
			return item.IsSeenOverrideValue;
		}
		set
		{
			ICollectionViewerItem item = this.Item;
			if (item == null || !item.IsVisibleInCollection())
			{
				return;
			}
			if (item.IsSeenOverridden)
			{
				item.IsSeenOverrideValue = value;
				return;
			}
			item.IsSeen = value;
		}
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00054E44 File Offset: 0x00053044
	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			componentInParent.OnPaneEnd += delegate()
			{
				this.Item = null;
			};
		}
		if (this.playingIndicator)
		{
			this.playingIndicator.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x00054E98 File Offset: 0x00053098
	public void SetItem(ICollectionViewerItem item, out bool isNew)
	{
		this.Item = item;
		this.UpdateDisplay();
		base.gameObject.name = ((item != null) ? item.name : "null");
		if (this.newBurst)
		{
			this.newBurst.SetActive(false);
		}
		if (item == null)
		{
			isNew = false;
			return;
		}
		if (this.currentIconOverride && this.currentIconOverride.Owner == this)
		{
			this.currentIconOverride.gameObject.SetActive(false);
			this.currentIconOverride = null;
		}
		this.iconTransform = (this.itemIcon ? this.itemIcon.transform : base.transform);
		if (item.IsVisibleInCollection())
		{
			isNew = false;
			CollectableRelic collectableRelic = item as CollectableRelic;
			if (collectableRelic != null && collectableRelic.RelicType && collectableRelic.RelicType.IconOverridePrefab)
			{
				if (!CollectionViewBoardItem._spawnedIconOverrides.ContainsKey(item) || CollectionViewBoardItem._spawnedIconOverrides[item] == null)
				{
					CustomInventoryItemCollectableDisplay customInventoryItemCollectableDisplay = Object.Instantiate<CustomInventoryItemCollectableDisplay>(collectableRelic.RelicType.IconOverridePrefab, this.iconTransform);
					customInventoryItemCollectableDisplay.transform.Reset();
					CollectionViewBoardItem._spawnedIconOverrides[item] = customInventoryItemCollectableDisplay;
					this.currentIconOverride = customInventoryItemCollectableDisplay;
				}
				else
				{
					this.currentIconOverride = CollectionViewBoardItem._spawnedIconOverrides[item];
					this.currentIconOverride.transform.SetParentReset(this.iconTransform);
					this.currentIconOverride.gameObject.SetActive(true);
				}
			}
			this.ShowItemIcon();
		}
		else
		{
			CollectableItemMemento collectableItemMemento = item as CollectableItemMemento;
			isNew = (collectableItemMemento != null && collectableItemMemento.CollectedAmount > 0);
			if (this.itemIcon)
			{
				this.itemIcon.AlphaSelf = 0f;
			}
			if (this.emptyNotch)
			{
				this.emptyNotch.SetActive(true);
			}
		}
		this.Board.UpdateRelicItemsIsPlaying();
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x00055081 File Offset: 0x00053281
	public override bool Cancel()
	{
		if (this.Canceled != null)
		{
			this.Canceled();
			return true;
		}
		return base.Cancel();
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x0005509E File Offset: 0x0005329E
	public override bool Extra()
	{
		if (this.Item == null)
		{
			return base.Extra();
		}
		if (this.Item.IsVisibleInCollection() && this.Board.TryPlayRelic(this))
		{
			this.Board.UpdateRelicItemsIsPlaying();
			return true;
		}
		return base.Extra();
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000550E0 File Offset: 0x000532E0
	public void UpdatedIsPlaying()
	{
		if (!this.currentIconOverride)
		{
			return;
		}
		bool flag = this.Board.IsPlaying(this);
		Animator component = this.currentIconOverride.GetComponent<Animator>();
		if (component)
		{
			component.SetBool(CollectionViewBoardItem._isPlayingAnim, flag);
		}
		if (this.playingIndicator)
		{
			if (flag && !this.playingIndicator.gameObject.activeSelf)
			{
				this.playingIndicator.gameObject.SetActive(true);
			}
			this.playingIndicator.SetBool(CollectionViewBoardItem._isPlayingAnim, flag);
		}
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x0005516C File Offset: 0x0005336C
	private void ShowItemIcon()
	{
		if (this.itemIcon)
		{
			this.itemIcon.Sprite = this.Item.GetCollectionIcon();
			if (this.currentIconOverride == null)
			{
				this.itemIcon.AlphaSelf = 1f;
			}
			else
			{
				this.itemIcon.AlphaSelf = 0f;
				SpriteRenderer component = this.currentIconOverride.GetComponent<SpriteRenderer>();
				if (component)
				{
					component.maskInteraction = this.itemIcon.GetComponent<SpriteRenderer>().maskInteraction;
				}
			}
		}
		if (this.emptyNotch)
		{
			this.emptyNotch.SetActive(false);
		}
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00055210 File Offset: 0x00053410
	public void DoNewAppear()
	{
		this.iconTransform.gameObject.SetActive(true);
		this.ShowItemIcon();
		this.UpdateDisplay();
		Animator animator = null;
		if (this.currentIconOverride)
		{
			animator = this.currentIconOverride.GetComponent<Animator>();
		}
		else if (this.itemIcon)
		{
			animator = this.itemIcon.GetComponent<Animator>();
		}
		if (animator != null)
		{
			animator.Play("Appear");
		}
		if (this.newBurst)
		{
			this.newBurst.SetActive(true);
		}
		this.newBurstSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		this.newBurstShake.DoShake(this, true);
	}

	// Token: 0x04001105 RID: 4357
	[SerializeField]
	private GameObject emptyNotch;

	// Token: 0x04001106 RID: 4358
	[SerializeField]
	private NestedFadeGroupSpriteRenderer itemIcon;

	// Token: 0x04001107 RID: 4359
	[SerializeField]
	private Animator playingIndicator;

	// Token: 0x04001108 RID: 4360
	[SerializeField]
	private GameObject newBurst;

	// Token: 0x04001109 RID: 4361
	[SerializeField]
	private AudioEvent newBurstSound;

	// Token: 0x0400110A RID: 4362
	[SerializeField]
	private CameraShakeTarget newBurstShake;

	// Token: 0x0400110B RID: 4363
	private CustomInventoryItemCollectableDisplay currentIconOverride;

	// Token: 0x0400110C RID: 4364
	private static readonly Dictionary<ICollectionViewerItem, CustomInventoryItemCollectableDisplay> _spawnedIconOverrides = new Dictionary<ICollectionViewerItem, CustomInventoryItemCollectableDisplay>();

	// Token: 0x0400110D RID: 4365
	private static readonly int _isPlayingAnim = Animator.StringToHash("IsPlaying");

	// Token: 0x0400110E RID: 4366
	private Transform iconTransform;
}
