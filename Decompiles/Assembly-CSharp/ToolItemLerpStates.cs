using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005E9 RID: 1513
[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Lerp States)")]
public class ToolItemLerpStates : ToolItem
{
	// Token: 0x17000623 RID: 1571
	// (get) Token: 0x060035E3 RID: 13795 RVA: 0x000ED7AB File Offset: 0x000EB9AB
	public override LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x17000624 RID: 1572
	// (get) Token: 0x060035E4 RID: 13796 RVA: 0x000ED7B3 File Offset: 0x000EB9B3
	public override LocalisedString Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x17000625 RID: 1573
	// (get) Token: 0x060035E5 RID: 13797 RVA: 0x000ED7BB File Offset: 0x000EB9BB
	public override ToolItem.UsageOptions Usage
	{
		get
		{
			return this.usageOptions;
		}
	}

	// Token: 0x060035E6 RID: 13798 RVA: 0x000ED7C4 File Offset: 0x000EB9C4
	public override Sprite GetInventorySprite(ToolItem.IconVariants iconVariant)
	{
		ToolItemLerpStates.StateInfo currentState = this.GetCurrentState();
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (currentState.InventorySpritePoison ? currentState.InventorySpritePoison : currentState.InventorySprite);
		}
		else
		{
			result = currentState.InventorySprite;
		}
		return result;
	}

	// Token: 0x060035E7 RID: 13799 RVA: 0x000ED804 File Offset: 0x000EBA04
	public override Sprite GetHudSprite(ToolItem.IconVariants iconVariant)
	{
		ToolItemLerpStates.StateInfo currentState = this.GetCurrentState();
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (currentState.HudSpritePoison ? currentState.HudSpritePoison : currentState.HudSprite);
		}
		else
		{
			result = currentState.HudSprite;
		}
		return result;
	}

	// Token: 0x060035E8 RID: 13800 RVA: 0x000ED844 File Offset: 0x000EBA44
	private ToolItemLerpStates.StateInfo GetCurrentState()
	{
		if (!Application.isPlaying)
		{
			return this.fullState;
		}
		int amountLeft = base.SavedData.AmountLeft;
		int toolStorageAmount = ToolItemManager.GetToolStorageAmount(this);
		if (amountLeft <= 0)
		{
			return this.emptyState;
		}
		if (amountLeft >= toolStorageAmount)
		{
			return this.fullState;
		}
		if (this.states == null || this.states.Length == 0)
		{
			return this.emptyState;
		}
		int num = Mathf.RoundToInt((float)amountLeft / (float)toolStorageAmount * (float)this.states.Length);
		if (num >= this.states.Length)
		{
			num = this.states.Length - 1;
		}
		return this.states[num];
	}

	// Token: 0x0400390C RID: 14604
	[Header("States")]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x0400390D RID: 14605
	[SerializeField]
	private LocalisedString description;

	// Token: 0x0400390E RID: 14606
	[Space]
	[SerializeField]
	private ToolItemLerpStates.StateInfo emptyState;

	// Token: 0x0400390F RID: 14607
	[SerializeField]
	private ToolItemLerpStates.StateInfo[] states;

	// Token: 0x04003910 RID: 14608
	[SerializeField]
	private ToolItemLerpStates.StateInfo fullState;

	// Token: 0x04003911 RID: 14609
	[Space]
	[SerializeField]
	private ToolItem.UsageOptions usageOptions;

	// Token: 0x020018F2 RID: 6386
	[Serializable]
	private struct StateInfo
	{
		// Token: 0x040093DD RID: 37853
		public Sprite InventorySprite;

		// Token: 0x040093DE RID: 37854
		public Sprite InventorySpritePoison;

		// Token: 0x040093DF RID: 37855
		public Sprite HudSprite;

		// Token: 0x040093E0 RID: 37856
		public Sprite HudSpritePoison;
	}
}
