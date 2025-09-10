using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020006A4 RID: 1700
public class InventoryItemSpoolPieces : InventoryItemSelectableDirectional
{
	// Token: 0x170006E9 RID: 1769
	// (get) Token: 0x06003CB0 RID: 15536 RVA: 0x0010A574 File Offset: 0x00108774
	public override string DisplayName
	{
		get
		{
			return this.currentState.DisplayName;
		}
	}

	// Token: 0x170006EA RID: 1770
	// (get) Token: 0x06003CB1 RID: 15537 RVA: 0x0010A586 File Offset: 0x00108786
	public override string Description
	{
		get
		{
			return this.currentState.Description;
		}
	}

	// Token: 0x06003CB2 RID: 15538 RVA: 0x0010A598 File Offset: 0x00108798
	protected override void Awake()
	{
		base.Awake();
		InventoryPane componentInParent = base.GetComponentInParent<InventoryPane>();
		if (componentInParent)
		{
			componentInParent.OnPaneStart += this.UpdateState;
		}
	}

	// Token: 0x06003CB3 RID: 15539 RVA: 0x0010A5CC File Offset: 0x001087CC
	protected override void Start()
	{
		base.Start();
		this.UpdateState();
	}

	// Token: 0x06003CB4 RID: 15540 RVA: 0x0010A5DC File Offset: 0x001087DC
	private void UpdateState()
	{
		PlayerData instance = PlayerData.instance;
		if (instance.silkSpoolParts <= 0 && instance.silkMax <= 9)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		if (instance.silkMax >= 18)
		{
			this.currentState = this.fullState;
		}
		else
		{
			int silkSpoolParts = instance.silkSpoolParts;
			if (silkSpoolParts != 0)
			{
				if (silkSpoolParts != 1)
				{
					this.currentState = this.fullState;
				}
				else
				{
					this.currentState = (CollectableItemManager.IsInHiddenMode() ? this.emptyState : this.halfState);
				}
			}
			else
			{
				this.currentState = this.emptyState;
			}
		}
		if (this.emptyState.DisplayObject)
		{
			this.emptyState.DisplayObject.SetActive(false);
		}
		if (this.halfState.DisplayObject)
		{
			this.halfState.DisplayObject.SetActive(false);
		}
		if (this.fullState.DisplayObject)
		{
			this.fullState.DisplayObject.SetActive(false);
		}
		if (this.currentState.DisplayObject)
		{
			this.currentState.DisplayObject.SetActive(true);
		}
	}

	// Token: 0x04003E70 RID: 15984
	[Space]
	[SerializeField]
	private InventoryItemSpoolPieces.DisplayState emptyState;

	// Token: 0x04003E71 RID: 15985
	[SerializeField]
	private InventoryItemSpoolPieces.DisplayState halfState;

	// Token: 0x04003E72 RID: 15986
	[SerializeField]
	private InventoryItemSpoolPieces.DisplayState fullState;

	// Token: 0x04003E73 RID: 15987
	private InventoryItemSpoolPieces.DisplayState currentState;

	// Token: 0x020019A1 RID: 6561
	[Serializable]
	private struct DisplayState
	{
		// Token: 0x04009675 RID: 38517
		public GameObject DisplayObject;

		// Token: 0x04009676 RID: 38518
		public LocalisedString DisplayName;

		// Token: 0x04009677 RID: 38519
		public LocalisedString Description;
	}
}
