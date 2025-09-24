using System;
using UnityEngine;

// Token: 0x0200069A RID: 1690
public class InventoryItemMaterium : InventoryItemUpdateable
{
	// Token: 0x170006D1 RID: 1745
	// (get) Token: 0x06003C50 RID: 15440 RVA: 0x001096CB File Offset: 0x001078CB
	// (set) Token: 0x06003C51 RID: 15441 RVA: 0x001096D3 File Offset: 0x001078D3
	public MateriumItem ItemData
	{
		get
		{
			return this.itemData;
		}
		set
		{
			this.itemData = value;
			this.Refresh();
		}
	}

	// Token: 0x170006D2 RID: 1746
	// (get) Token: 0x06003C52 RID: 15442 RVA: 0x001096E2 File Offset: 0x001078E2
	public override string DisplayName
	{
		get
		{
			return this.ItemData.DisplayName;
		}
	}

	// Token: 0x170006D3 RID: 1747
	// (get) Token: 0x06003C53 RID: 15443 RVA: 0x001096F4 File Offset: 0x001078F4
	public override string Description
	{
		get
		{
			return this.ItemData.Description;
		}
	}

	// Token: 0x170006D4 RID: 1748
	// (get) Token: 0x06003C54 RID: 15444 RVA: 0x00109706 File Offset: 0x00107906
	public Sprite Sprite
	{
		get
		{
			return this.ItemData.Icon;
		}
	}

	// Token: 0x170006D5 RID: 1749
	// (get) Token: 0x06003C55 RID: 15445 RVA: 0x00109713 File Offset: 0x00107913
	// (set) Token: 0x06003C56 RID: 15446 RVA: 0x00109716 File Offset: 0x00107916
	protected override bool IsSeen
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	// Token: 0x06003C57 RID: 15447 RVA: 0x00109718 File Offset: 0x00107918
	protected override void Awake()
	{
		base.Awake();
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			componentInParent.OnPaneStart += this.Refresh;
		}
		this.Refresh();
	}

	// Token: 0x06003C58 RID: 15448 RVA: 0x00109760 File Offset: 0x00107960
	private void Refresh()
	{
		bool flag = this.itemData && this.itemData.IsCollected;
		if (this.spriteRenderer)
		{
			this.spriteRenderer.sprite = (flag ? this.Sprite : this.emptySprite);
		}
	}

	// Token: 0x04003E50 RID: 15952
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04003E51 RID: 15953
	[SerializeField]
	private Sprite emptySprite;

	// Token: 0x04003E52 RID: 15954
	private MateriumItem itemData;
}
