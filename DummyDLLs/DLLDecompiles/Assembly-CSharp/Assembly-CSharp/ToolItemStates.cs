using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (States)")]
public class ToolItemStates : ToolItem
{
	// Token: 0x17000634 RID: 1588
	// (get) Token: 0x0600364F RID: 13903 RVA: 0x000F01C7 File Offset: 0x000EE3C7
	public override LocalisedString DisplayName
	{
		get
		{
			return this.GetCurrentStateSafe().DisplayName;
		}
	}

	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x06003650 RID: 13904 RVA: 0x000F01D4 File Offset: 0x000EE3D4
	public override LocalisedString Description
	{
		get
		{
			return this.GetCurrentStateSafe().Description;
		}
	}

	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x06003651 RID: 13905 RVA: 0x000F01E1 File Offset: 0x000EE3E1
	public override ToolItem.UsageOptions Usage
	{
		get
		{
			return this.GetCurrentStateSafe().Usage;
		}
	}

	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x06003652 RID: 13906 RVA: 0x000F01F0 File Offset: 0x000EE3F0
	public override bool UsableWhenEmpty
	{
		get
		{
			ToolItem.UsageOptions usageOptions = this.hasUsableEmptyState ? this.usableEmptyState.Usage : this.emptyState.Usage;
			return usageOptions.ThrowPrefab != null || !string.IsNullOrEmpty(usageOptions.FsmEventName);
		}
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x000F023C File Offset: 0x000EE43C
	public override Sprite GetInventorySprite(ToolItem.IconVariants iconVariant)
	{
		ToolItemStates.StateInfo currentStateSafe = this.GetCurrentStateSafe();
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (currentStateSafe.InventorySpritePoison ? currentStateSafe.InventorySpritePoison : currentStateSafe.InventorySprite);
		}
		else
		{
			result = currentStateSafe.InventorySprite;
		}
		return result;
	}

	// Token: 0x06003654 RID: 13908 RVA: 0x000F027C File Offset: 0x000EE47C
	public override Sprite GetHudSprite(ToolItem.IconVariants iconVariant)
	{
		ToolItemStates.StateInfo currentStateSafe = this.GetCurrentStateSafe();
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (currentStateSafe.HudSpritePoison ? currentStateSafe.HudSpritePoison : currentStateSafe.HudSprite);
		}
		else
		{
			result = currentStateSafe.HudSprite;
		}
		return result;
	}

	// Token: 0x06003655 RID: 13909 RVA: 0x000F02BA File Offset: 0x000EE4BA
	private ToolItemStates.StateInfo GetCurrentStateSafe()
	{
		if (!Application.isPlaying)
		{
			return this.fullState;
		}
		return this.GetCurrentState();
	}

	// Token: 0x06003656 RID: 13910 RVA: 0x000F02D0 File Offset: 0x000EE4D0
	protected ToolItemStates.StateInfo GetCurrentState()
	{
		if (!base.IsEmpty)
		{
			return this.fullState;
		}
		if (this.hasUsableEmptyState && this.UsableWhenEmpty)
		{
			return this.usableEmptyState;
		}
		return this.emptyState;
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x000F02FE File Offset: 0x000EE4FE
	protected override Sprite GetFullIcon()
	{
		return this.fullState.InventorySprite;
	}

	// Token: 0x0400394D RID: 14669
	[Header("States")]
	[SerializeField]
	private ToolItemStates.StateInfo fullState;

	// Token: 0x0400394E RID: 14670
	[SerializeField]
	private bool hasUsableEmptyState;

	// Token: 0x0400394F RID: 14671
	[SerializeField]
	private ToolItemStates.StateInfo usableEmptyState;

	// Token: 0x04003950 RID: 14672
	[SerializeField]
	private ToolItemStates.StateInfo emptyState;

	// Token: 0x020018FD RID: 6397
	[Serializable]
	protected struct StateInfo
	{
		// Token: 0x0400940A RID: 37898
		public LocalisedString DisplayName;

		// Token: 0x0400940B RID: 37899
		public LocalisedString Description;

		// Token: 0x0400940C RID: 37900
		public Sprite InventorySprite;

		// Token: 0x0400940D RID: 37901
		public Sprite InventorySpritePoison;

		// Token: 0x0400940E RID: 37902
		public Sprite HudSprite;

		// Token: 0x0400940F RID: 37903
		public Sprite HudSpritePoison;

		// Token: 0x04009410 RID: 37904
		public ToolItem.UsageOptions Usage;
	}
}
