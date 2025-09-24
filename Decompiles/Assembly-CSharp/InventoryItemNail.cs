using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200069C RID: 1692
public class InventoryItemNail : InventoryItemUpdateable
{
	// Token: 0x170006D6 RID: 1750
	// (get) Token: 0x06003C60 RID: 15456 RVA: 0x00109919 File Offset: 0x00107B19
	public override string DisplayName
	{
		get
		{
			return this.currentState.DisplayName;
		}
	}

	// Token: 0x170006D7 RID: 1751
	// (get) Token: 0x06003C61 RID: 15457 RVA: 0x0010992B File Offset: 0x00107B2B
	public override string Description
	{
		get
		{
			return this.currentState.Description;
		}
	}

	// Token: 0x170006D8 RID: 1752
	// (get) Token: 0x06003C62 RID: 15458 RVA: 0x0010993D File Offset: 0x00107B3D
	// (set) Token: 0x06003C63 RID: 15459 RVA: 0x0010994C File Offset: 0x00107B4C
	protected override bool IsSeen
	{
		get
		{
			return !PlayerData.instance.InvNailHasNew;
		}
		set
		{
			PlayerData.instance.InvNailHasNew = !value;
		}
	}

	// Token: 0x06003C64 RID: 15460 RVA: 0x0010995C File Offset: 0x00107B5C
	protected override void Awake()
	{
		base.Awake();
		InventoryPane componentInParent = base.GetComponentInParent<InventoryPane>();
		if (componentInParent)
		{
			componentInParent.OnPaneStart += this.UpdateState;
		}
	}

	// Token: 0x06003C65 RID: 15461 RVA: 0x00109990 File Offset: 0x00107B90
	protected override void Start()
	{
		base.Start();
		this.UpdateState();
	}

	// Token: 0x06003C66 RID: 15462 RVA: 0x001099A0 File Offset: 0x00107BA0
	private void UpdateState()
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		foreach (InventoryItemNail.DisplayState displayState in this.displayStates)
		{
			if (displayState.DisplayObject)
			{
				displayState.DisplayObject.SetActive(false);
			}
		}
		int num = PlayerData.instance.nailUpgrades;
		int num2 = this.displayStates.Length - 1;
		if (num > num2)
		{
			num = num2;
		}
		this.currentState = this.displayStates[num];
		if (this.currentState.DisplayObject)
		{
			this.currentState.DisplayObject.SetActive(true);
		}
	}

	// Token: 0x04003E54 RID: 15956
	[Space]
	[SerializeField]
	private InventoryItemNail.DisplayState[] displayStates;

	// Token: 0x04003E55 RID: 15957
	private InventoryItemNail.DisplayState currentState;

	// Token: 0x0200199E RID: 6558
	[Serializable]
	private class DisplayState
	{
		// Token: 0x0400966F RID: 38511
		public GameObject DisplayObject;

		// Token: 0x04009670 RID: 38512
		public LocalisedString DisplayName;

		// Token: 0x04009671 RID: 38513
		public LocalisedString Description;
	}
}
