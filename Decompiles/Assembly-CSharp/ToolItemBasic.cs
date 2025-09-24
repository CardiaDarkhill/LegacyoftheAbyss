using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Basic)")]
public class ToolItemBasic : ToolItem
{
	// Token: 0x17000620 RID: 1568
	// (get) Token: 0x060035DB RID: 13787 RVA: 0x000ED6DD File Offset: 0x000EB8DD
	public override LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x17000621 RID: 1569
	// (get) Token: 0x060035DC RID: 13788 RVA: 0x000ED6E5 File Offset: 0x000EB8E5
	public override LocalisedString Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x17000622 RID: 1570
	// (get) Token: 0x060035DD RID: 13789 RVA: 0x000ED6ED File Offset: 0x000EB8ED
	public override ToolItem.UsageOptions Usage
	{
		get
		{
			return this.usageOptions;
		}
	}

	// Token: 0x060035DE RID: 13790 RVA: 0x000ED6F5 File Offset: 0x000EB8F5
	public override string GetPopupName()
	{
		if (this.popupNameOverride.IsEmpty)
		{
			return base.GetPopupName();
		}
		return this.popupNameOverride;
	}

	// Token: 0x060035DF RID: 13791 RVA: 0x000ED716 File Offset: 0x000EB916
	public override Sprite GetPopupIcon()
	{
		if (!this.popupIconOverride)
		{
			return base.GetPopupIcon();
		}
		return this.popupIconOverride;
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x000ED734 File Offset: 0x000EB934
	public override Sprite GetInventorySprite(ToolItem.IconVariants iconVariant)
	{
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (this.inventorySpritePoison ? this.inventorySpritePoison : this.inventorySprite);
		}
		else
		{
			result = this.inventorySprite;
		}
		return result;
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x000ED76C File Offset: 0x000EB96C
	public override Sprite GetHudSprite(ToolItem.IconVariants iconVariant)
	{
		Sprite result;
		if (iconVariant == ToolItem.IconVariants.Poison)
		{
			result = (this.hudSpritePoison ? this.hudSpritePoison : this.hudSprite);
		}
		else
		{
			result = this.hudSprite;
		}
		return result;
	}

	// Token: 0x04003903 RID: 14595
	[Header("Basic")]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x04003904 RID: 14596
	[SerializeField]
	private LocalisedString description;

	// Token: 0x04003905 RID: 14597
	[SerializeField]
	private Sprite inventorySprite;

	// Token: 0x04003906 RID: 14598
	[SerializeField]
	private Sprite inventorySpritePoison;

	// Token: 0x04003907 RID: 14599
	[SerializeField]
	private Sprite hudSprite;

	// Token: 0x04003908 RID: 14600
	[SerializeField]
	private Sprite hudSpritePoison;

	// Token: 0x04003909 RID: 14601
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString popupNameOverride;

	// Token: 0x0400390A RID: 14602
	[SerializeField]
	private Sprite popupIconOverride;

	// Token: 0x0400390B RID: 14603
	[Space]
	[SerializeField]
	private ToolItem.UsageOptions usageOptions;
}
