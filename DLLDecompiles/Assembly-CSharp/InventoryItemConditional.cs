using System;
using UnityEngine;

// Token: 0x0200068B RID: 1675
public class InventoryItemConditional : InventoryItemBasic
{
	// Token: 0x170006C1 RID: 1729
	// (get) Token: 0x06003BC3 RID: 15299 RVA: 0x00106C6F File Offset: 0x00104E6F
	public override string DisplayName
	{
		get
		{
			if (!this.GetObj().activeSelf)
			{
				return string.Empty;
			}
			return base.DisplayName;
		}
	}

	// Token: 0x170006C2 RID: 1730
	// (get) Token: 0x06003BC4 RID: 15300 RVA: 0x00106C8A File Offset: 0x00104E8A
	public override string Description
	{
		get
		{
			if (!this.GetObj().activeSelf)
			{
				return string.Empty;
			}
			return base.Description;
		}
	}

	// Token: 0x06003BC5 RID: 15301 RVA: 0x00106CA8 File Offset: 0x00104EA8
	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			componentInParent.OnPaneStart += this.Evaluate;
		}
		this.buttonPrompt = base.GetComponent<InventoryItemButtonPrompt>();
		this.Evaluate();
	}

	// Token: 0x06003BC6 RID: 15302 RVA: 0x00106CF0 File Offset: 0x00104EF0
	private void Evaluate()
	{
		bool flag = (!this.hideWhenInventoryBare || !CollectableItemManager.IsInHiddenMode()) && this.Test.IsFulfilled;
		this.GetObj().SetActive(flag);
		if (this.buttonPrompt)
		{
			this.buttonPrompt.enabled = flag;
		}
	}

	// Token: 0x06003BC7 RID: 15303 RVA: 0x00106D41 File Offset: 0x00104F41
	private GameObject GetObj()
	{
		if (!this.overrideSetActive)
		{
			return base.gameObject;
		}
		return this.overrideSetActive;
	}

	// Token: 0x04003DF3 RID: 15859
	[Space]
	public PlayerDataTest Test;

	// Token: 0x04003DF4 RID: 15860
	[SerializeField]
	private bool hideWhenInventoryBare;

	// Token: 0x04003DF5 RID: 15861
	[SerializeField]
	private GameObject overrideSetActive;

	// Token: 0x04003DF6 RID: 15862
	private InventoryItemButtonPrompt buttonPrompt;
}
