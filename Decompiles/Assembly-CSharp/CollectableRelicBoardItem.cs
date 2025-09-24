using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class CollectableRelicBoardItem : InventoryItemSelectableDirectional, InventoryItemListManager<CollectableRelicBoardItem, CollectableRelic>.IMoveNextButton
{
	// Token: 0x14000039 RID: 57
	// (add) Token: 0x060012FE RID: 4862 RVA: 0x00057688 File Offset: 0x00055888
	// (remove) Token: 0x060012FF RID: 4863 RVA: 0x000576C0 File Offset: 0x000558C0
	public event Action Canceled;

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001300 RID: 4864 RVA: 0x000576F5 File Offset: 0x000558F5
	// (set) Token: 0x06001301 RID: 4865 RVA: 0x000576FD File Offset: 0x000558FD
	public CollectableRelic Relic { get; private set; }

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001302 RID: 4866 RVA: 0x00057706 File Offset: 0x00055906
	public override string DisplayName
	{
		get
		{
			if (!this.Relic || !this.Relic.SavedData.IsDeposited)
			{
				return string.Empty;
			}
			return this.Relic.DisplayName;
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001303 RID: 4867 RVA: 0x00057738 File Offset: 0x00055938
	public override string Description
	{
		get
		{
			if (!this.Relic || !this.Relic.SavedData.IsDeposited)
			{
				return string.Empty;
			}
			return this.Relic.Description;
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001304 RID: 4868 RVA: 0x0005776A File Offset: 0x0005596A
	// (set) Token: 0x06001305 RID: 4869 RVA: 0x00057772 File Offset: 0x00055972
	public CollectableRelicBoard Board { get; set; }

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06001306 RID: 4870 RVA: 0x0005777B File Offset: 0x0005597B
	public bool WillSubmitMoveNext
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x00057780 File Offset: 0x00055980
	protected override void Awake()
	{
		base.Awake();
		if (this.newOrb)
		{
			this.newOrbInitialScale = this.newOrb.transform.localScale;
		}
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			componentInParent.OnPaneEnd += delegate()
			{
				this.MarkAsSeen();
				this.Relic = null;
				if (this.newOrb)
				{
					this.newOrb.transform.localScale = this.newOrbInitialScale;
				}
			};
		}
		if (this.playingIndicator)
		{
			this.playingIndicator.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x000577F8 File Offset: 0x000559F8
	public void SetRelic(CollectableRelic relic, out bool isNew)
	{
		this.Relic = relic;
		base.gameObject.name = (relic ? relic.name : "null");
		isNew = false;
		if (!relic)
		{
			return;
		}
		CollectableItemRelicType relicType = relic.RelicType;
		if (!relicType)
		{
			return;
		}
		CollectableRelicsData.Data savedData = relic.SavedData;
		if (this.currentIconOverride && this.currentIconOverride.Owner == this)
		{
			this.currentIconOverride.gameObject.SetActive(false);
			this.currentIconOverride = null;
		}
		this.iconTransform = (this.relicIcon ? this.relicIcon.transform : base.transform);
		if (savedData.IsDeposited)
		{
			if (relic.RelicType && relic.RelicType.IconOverridePrefab)
			{
				if (!CollectableRelicBoardItem._spawnedIconOverrides.ContainsKey(relic) || CollectableRelicBoardItem._spawnedIconOverrides[relic] == null)
				{
					CustomInventoryItemCollectableDisplay customInventoryItemCollectableDisplay = Object.Instantiate<CustomInventoryItemCollectableDisplay>(relic.RelicType.IconOverridePrefab, this.iconTransform);
					customInventoryItemCollectableDisplay.transform.Reset();
					CollectableRelicBoardItem._spawnedIconOverrides[relic] = customInventoryItemCollectableDisplay;
					this.currentIconOverride = customInventoryItemCollectableDisplay;
				}
				else
				{
					this.currentIconOverride = CollectableRelicBoardItem._spawnedIconOverrides[relic];
					this.currentIconOverride.transform.SetParentReset(this.iconTransform);
					this.currentIconOverride.gameObject.SetActive(true);
				}
			}
			if (this.relicIcon)
			{
				this.relicIcon.Sprite = relicType.GetIcon(CollectableItem.ReadSource.Inventory);
				this.relicIcon.AlphaSelf = ((!this.currentIconOverride) ? 1f : 0f);
			}
			this.isNewOrbVisible = !savedData.HasSeenInRelicBoard;
			if (this.emptyNotch)
			{
				this.emptyNotch.SetActive(false);
			}
		}
		else
		{
			if (this.relicIcon)
			{
				this.relicIcon.AlphaSelf = 0f;
			}
			this.isNewOrbVisible = false;
			if (this.emptyNotch)
			{
				this.emptyNotch.SetActive(true);
			}
		}
		if (this.newBurst)
		{
			this.newBurst.SetActive(false);
		}
		this.Board.UpdateRelicItemsIsPlaying();
		if (this.isNewOrbVisible)
		{
			isNew = true;
			this.isNewOrbVisible = false;
			this.iconTransform.gameObject.SetActive(false);
			if (this.emptyNotch)
			{
				this.emptyNotch.SetActive(true);
			}
		}
		if (this.newOrb)
		{
			this.newOrb.SetActive(this.isNewOrbVisible);
		}
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00057A94 File Offset: 0x00055C94
	public void DoNewAppear()
	{
		this.iconTransform.gameObject.SetActive(true);
		if (this.emptyNotch)
		{
			this.emptyNotch.SetActive(false);
		}
		Animator animator = null;
		if (this.currentIconOverride)
		{
			animator = this.currentIconOverride.GetComponent<Animator>();
		}
		else if (this.relicIcon)
		{
			animator = this.relicIcon.GetComponent<Animator>();
		}
		if (animator != null)
		{
			animator.Play("Appear");
		}
		this.isNewOrbVisible = true;
		if (this.newOrb)
		{
			this.newOrb.SetActive(true);
		}
		if (this.newBurst)
		{
			this.newBurst.SetActive(true);
		}
		this.newBurstSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		this.newBurstShake.DoShake(this, true);
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00057B78 File Offset: 0x00055D78
	public override bool Cancel()
	{
		if (this.Canceled != null)
		{
			this.Canceled();
			return true;
		}
		return base.Cancel();
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00057B98 File Offset: 0x00055D98
	public override bool Extra()
	{
		if (!this.Relic)
		{
			return base.Extra();
		}
		if (this.Relic.SavedData.IsDeposited && this.Board.TryPlayRelic(this))
		{
			this.Board.UpdateRelicItemsIsPlaying();
			return true;
		}
		return base.Extra();
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00057BEC File Offset: 0x00055DEC
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		if (this.isNewOrbVisible)
		{
			this.isNewOrbVisible = false;
			this.MarkAsSeen();
			if (this.newOrb)
			{
				this.newOrb.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, UI.NewDotScaleDelay, false, false, null);
			}
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00057C48 File Offset: 0x00055E48
	private void MarkAsSeen()
	{
		if (!this.Relic)
		{
			return;
		}
		CollectableRelicsData.Data savedData = this.Relic.SavedData;
		if (!savedData.IsDeposited)
		{
			return;
		}
		if (!savedData.HasSeenInRelicBoard)
		{
			savedData.HasSeenInRelicBoard = true;
			this.Relic.SavedData = savedData;
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00057C94 File Offset: 0x00055E94
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
			component.SetBool(CollectableRelicBoardItem._isPlayingAnim, flag);
		}
		if (this.playingIndicator)
		{
			if (flag && !this.playingIndicator.gameObject.activeSelf)
			{
				this.playingIndicator.gameObject.SetActive(true);
			}
			this.playingIndicator.SetBool(CollectableRelicBoardItem._isPlayingAnim, flag);
		}
	}

	// Token: 0x04001199 RID: 4505
	[SerializeField]
	private GameObject emptyNotch;

	// Token: 0x0400119A RID: 4506
	[SerializeField]
	private NestedFadeGroupSpriteRenderer relicIcon;

	// Token: 0x0400119B RID: 4507
	[SerializeField]
	private GameObject newOrb;

	// Token: 0x0400119C RID: 4508
	[SerializeField]
	private GameObject newBurst;

	// Token: 0x0400119D RID: 4509
	[SerializeField]
	private AudioEvent newBurstSound;

	// Token: 0x0400119E RID: 4510
	[SerializeField]
	private CameraShakeTarget newBurstShake;

	// Token: 0x0400119F RID: 4511
	[SerializeField]
	private Animator playingIndicator;

	// Token: 0x040011A0 RID: 4512
	private bool isNewOrbVisible;

	// Token: 0x040011A1 RID: 4513
	private Vector3 newOrbInitialScale;

	// Token: 0x040011A2 RID: 4514
	private CustomInventoryItemCollectableDisplay currentIconOverride;

	// Token: 0x040011A3 RID: 4515
	private static readonly Dictionary<CollectableRelic, CustomInventoryItemCollectableDisplay> _spawnedIconOverrides = new Dictionary<CollectableRelic, CustomInventoryItemCollectableDisplay>();

	// Token: 0x040011A4 RID: 4516
	private static readonly int _isPlayingAnim = Animator.StringToHash("IsPlaying");

	// Token: 0x040011A5 RID: 4517
	private Transform iconTransform;
}
