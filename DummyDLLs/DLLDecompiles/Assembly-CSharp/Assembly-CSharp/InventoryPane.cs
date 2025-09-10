using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006AD RID: 1709
public class InventoryPane : InventoryPaneBase
{
	// Token: 0x140000CC RID: 204
	// (add) Token: 0x06003D70 RID: 15728 RVA: 0x0010E5F0 File Offset: 0x0010C7F0
	// (remove) Token: 0x06003D71 RID: 15729 RVA: 0x0010E628 File Offset: 0x0010C828
	public event Action<bool> NewItemsUpdated;

	// Token: 0x1700070A RID: 1802
	// (get) Token: 0x06003D72 RID: 15730 RVA: 0x0010E65D File Offset: 0x0010C85D
	public string DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x1700070B RID: 1803
	// (get) Token: 0x06003D73 RID: 15731 RVA: 0x0010E66A File Offset: 0x0010C86A
	public Sprite ListIcon
	{
		get
		{
			return this.listIcon;
		}
	}

	// Token: 0x1700070C RID: 1804
	// (get) Token: 0x06003D74 RID: 15732 RVA: 0x0010E672 File Offset: 0x0010C872
	public virtual bool IsAvailable
	{
		get
		{
			if (this.playerDataTest.IsFulfilled)
			{
				if (this.availabilityProvider == null)
				{
					this.availabilityProvider = base.GetComponent<IInventoryPaneAvailabilityProvider>();
				}
				return this.availabilityProvider == null || this.availabilityProvider.IsAvailable();
			}
			return false;
		}
	}

	// Token: 0x1700070D RID: 1805
	// (get) Token: 0x06003D75 RID: 15733 RVA: 0x0010E6AC File Offset: 0x0010C8AC
	// (set) Token: 0x06003D76 RID: 15734 RVA: 0x0010E6CD File Offset: 0x0010C8CD
	public bool IsAnyUpdates
	{
		get
		{
			return !string.IsNullOrEmpty(this.hasNewPDBool) && PlayerData.instance.GetVariable(this.hasNewPDBool);
		}
		set
		{
			if (string.IsNullOrEmpty(this.hasNewPDBool))
			{
				return;
			}
			PlayerData.instance.SetVariable(this.hasNewPDBool, value);
		}
	}

	// Token: 0x1700070E RID: 1806
	// (get) Token: 0x06003D77 RID: 15735 RVA: 0x0010E6EE File Offset: 0x0010C8EE
	// (set) Token: 0x06003D78 RID: 15736 RVA: 0x0010E6F6 File Offset: 0x0010C8F6
	public InventoryPane RootPane { get; set; }

	// Token: 0x06003D79 RID: 15737 RVA: 0x0010E6FF File Offset: 0x0010C8FF
	protected virtual void Awake()
	{
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x0010E701 File Offset: 0x0010C901
	public override void PaneStart()
	{
		base.PaneStart();
		if (this.IsAnyUpdates)
		{
			this.IsAnyUpdates = false;
			if (this.NewItemsUpdated != null)
			{
				this.NewItemsUpdated(false);
			}
		}
	}

	// Token: 0x06003D7B RID: 15739 RVA: 0x0010E72C File Offset: 0x0010C92C
	public virtual InventoryPane Get()
	{
		return this;
	}

	// Token: 0x04003F2D RID: 16173
	[SerializeField]
	[FormerlySerializedAs("DisplayName")]
	private LocalisedString displayName;

	// Token: 0x04003F2E RID: 16174
	[SerializeField]
	private Sprite listIcon;

	// Token: 0x04003F2F RID: 16175
	[SerializeField]
	[FormerlySerializedAs("PlayerDataTest")]
	private PlayerDataTest playerDataTest;

	// Token: 0x04003F30 RID: 16176
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string hasNewPDBool;

	// Token: 0x04003F31 RID: 16177
	private int newItemsCount;

	// Token: 0x04003F32 RID: 16178
	private IInventoryPaneAvailabilityProvider availabilityProvider;
}
