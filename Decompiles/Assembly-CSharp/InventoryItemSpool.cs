using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020006A3 RID: 1699
public class InventoryItemSpool : InventoryItemBasic
{
	// Token: 0x170006E7 RID: 1767
	// (get) Token: 0x06003CAB RID: 15531 RVA: 0x0010A417 File Offset: 0x00108617
	public override string Description
	{
		get
		{
			if (PlayerData.instance.silkRegenMax <= 0)
			{
				return base.Description;
			}
			return this.heartVerDesc;
		}
	}

	// Token: 0x170006E8 RID: 1768
	// (get) Token: 0x06003CAC RID: 15532 RVA: 0x0010A438 File Offset: 0x00108638
	// (set) Token: 0x06003CAD RID: 15533 RVA: 0x0010A44F File Offset: 0x0010864F
	protected override bool IsSeen
	{
		get
		{
			return PlayerData.instance.silkRegenMax <= 0 || base.IsSeen;
		}
		set
		{
			if (PlayerData.instance.silkRegenMax <= 0)
			{
				return;
			}
			base.IsSeen = value;
		}
	}

	// Token: 0x06003CAE RID: 15534 RVA: 0x0010A468 File Offset: 0x00108668
	protected override void UpdateDisplay()
	{
		base.UpdateDisplay();
		PlayerData instance = PlayerData.instance;
		this.wideVer.SetActive(false);
		this.slimVer.SetActive(false);
		this.heartVer.SetActive(false);
		if (instance.silkRegenMax > 0)
		{
			this.heartVer.SetActive(true);
			for (int i = 0; i < this.hearts.Length; i++)
			{
				this.hearts[i].SetActive(instance.silkRegenMax > i);
			}
			return;
		}
		bool flag = false;
		foreach (object obj in this.conditionalParent)
		{
			InventoryItemConditional component = ((Transform)obj).GetComponent<InventoryItemConditional>();
			if (component && component.Test.IsFulfilled)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.slimVer.SetActive(true);
			return;
		}
		this.wideVer.SetActive(true);
	}

	// Token: 0x04003E6A RID: 15978
	[SerializeField]
	private LocalisedString heartVerDesc;

	// Token: 0x04003E6B RID: 15979
	[Space]
	[SerializeField]
	private GameObject wideVer;

	// Token: 0x04003E6C RID: 15980
	[SerializeField]
	private GameObject slimVer;

	// Token: 0x04003E6D RID: 15981
	[SerializeField]
	private GameObject heartVer;

	// Token: 0x04003E6E RID: 15982
	[SerializeField]
	private GameObject[] hearts;

	// Token: 0x04003E6F RID: 15983
	[SerializeField]
	private Transform conditionalParent;
}
