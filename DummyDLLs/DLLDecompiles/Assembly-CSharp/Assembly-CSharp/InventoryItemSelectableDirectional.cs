using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006A2 RID: 1698
public class InventoryItemSelectableDirectional : InventoryItemSelectable
{
	// Token: 0x170006E3 RID: 1763
	// (get) Token: 0x06003C97 RID: 15511 RVA: 0x0010A15A File Offset: 0x0010835A
	protected virtual bool IsAutoNavSelectable
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170006E4 RID: 1764
	// (get) Token: 0x06003C98 RID: 15512 RVA: 0x0010A15D File Offset: 0x0010835D
	// (set) Token: 0x06003C99 RID: 15513 RVA: 0x0010A165 File Offset: 0x00108365
	public InventoryItemGrid Grid { get; set; }

	// Token: 0x170006E5 RID: 1765
	// (get) Token: 0x06003C9A RID: 15514 RVA: 0x0010A16E File Offset: 0x0010836E
	// (set) Token: 0x06003C9B RID: 15515 RVA: 0x0010A176 File Offset: 0x00108376
	public int GridSectionIndex { get; set; }

	// Token: 0x170006E6 RID: 1766
	// (get) Token: 0x06003C9C RID: 15516 RVA: 0x0010A17F File Offset: 0x0010837F
	// (set) Token: 0x06003C9D RID: 15517 RVA: 0x0010A187 File Offset: 0x00108387
	public int GridItemIndex { get; set; }

	// Token: 0x06003C9E RID: 15518 RVA: 0x0010A190 File Offset: 0x00108390
	protected virtual void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<InventoryItemSelectable>(ref this.Selectables, typeof(InventoryItemManager.SelectionDirection));
		ArrayForEnumAttribute.EnsureArraySize<InventoryItemSelectableDirectional.SelectableList>(ref this.FallbackSelectables, typeof(InventoryItemManager.SelectionDirection));
		ArrayForEnumAttribute.EnsureArraySize<InventoryItemSelectableDirectional.SelectableList>(ref this.nextPages, typeof(InventoryItemManager.SelectionDirection));
	}

	// Token: 0x06003C9F RID: 15519 RVA: 0x0010A1DC File Offset: 0x001083DC
	protected virtual void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06003CA0 RID: 15520 RVA: 0x0010A1E4 File Offset: 0x001083E4
	protected virtual void OnEnable()
	{
		this.EvaluateAutoNav();
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x0010A1EC File Offset: 0x001083EC
	protected virtual void Start()
	{
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x0010A1EE File Offset: 0x001083EE
	protected virtual void OnDisable()
	{
		if (this.autoNavGroup)
		{
			this.autoNavGroup.Deregister(this);
		}
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x0010A20C File Offset: 0x0010840C
	protected void EvaluateAutoNav()
	{
		if (!this.autoNavGroup)
		{
			this.autoNavGroup = base.GetComponentInParent<InventoryAutoNavGroup>();
		}
		if (!this.autoNavGroup)
		{
			return;
		}
		if (this.IsAutoNavSelectable)
		{
			this.autoNavGroup.Register(this);
			return;
		}
		this.autoNavGroup.Deregister(this);
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x0010A261 File Offset: 0x00108461
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (!base.gameObject.activeSelf && direction != null)
		{
			return this.GetNextSelectable(direction.Value);
		}
		return base.Get(direction);
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x0010A28E File Offset: 0x0010848E
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return this.GetNextSelectable(direction, true);
	}

	// Token: 0x06003CA6 RID: 15526 RVA: 0x0010A298 File Offset: 0x00108498
	protected InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction, bool allowAutoNavOnFirst)
	{
		InventoryItemSelectable inventoryItemSelectable = this.Selectables[(int)direction];
		bool flag = !inventoryItemSelectable;
		if (inventoryItemSelectable == null || !inventoryItemSelectable.gameObject.activeInHierarchy)
		{
			inventoryItemSelectable = this.GetNextFallbackSelectable(direction);
		}
		if (inventoryItemSelectable == null && (!flag || allowAutoNavOnFirst))
		{
			inventoryItemSelectable = this.GetSelectableFromAutoNavGroup<InventoryItemSelectable>(direction, null);
			if (inventoryItemSelectable == this)
			{
				inventoryItemSelectable = null;
			}
		}
		if (inventoryItemSelectable == null && base.transform.parent)
		{
			IInventorySelectionParent componentInParent = base.transform.parent.GetComponentInParent<IInventorySelectionParent>();
			if (componentInParent != null)
			{
				inventoryItemSelectable = componentInParent.GetNextSelectable(this, new InventoryItemManager.SelectionDirection?(direction));
			}
		}
		return inventoryItemSelectable;
	}

	// Token: 0x06003CA7 RID: 15527 RVA: 0x0010A339 File Offset: 0x00108539
	protected InventoryItemSelectable GetNextFallbackSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return this.FallbackSelectables[(int)direction].Selectables.FirstOrDefault((InventoryItemSelectable fallback) => fallback != null && fallback.gameObject.activeInHierarchy);
	}

	// Token: 0x06003CA8 RID: 15528 RVA: 0x0010A36C File Offset: 0x0010856C
	public override InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectableDirectional.SelectableList selectableList = this.nextPages[(int)direction];
		if (selectableList != null)
		{
			foreach (InventoryItemSelectable inventoryItemSelectable in selectableList.Selectables)
			{
				if (!(inventoryItemSelectable == null) && inventoryItemSelectable.isActiveAndEnabled)
				{
					return inventoryItemSelectable;
				}
			}
		}
		return base.GetNextSelectablePage(currentSelected, direction);
	}

	// Token: 0x06003CA9 RID: 15529 RVA: 0x0010A3E4 File Offset: 0x001085E4
	protected InventoryItemSelectable GetSelectableFromAutoNavGroup<T>(InventoryItemManager.SelectionDirection direction, Func<T, bool> predicate = null) where T : InventoryItemSelectable
	{
		if (!this.autoNavGroup)
		{
			return this;
		}
		return this.autoNavGroup.GetNextSelectable<T>(this, direction, predicate);
	}

	// Token: 0x04003E63 RID: 15971
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	public InventoryItemSelectable[] Selectables;

	// Token: 0x04003E64 RID: 15972
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	[FormerlySerializedAs("fallbackSelectables")]
	public InventoryItemSelectableDirectional.SelectableList[] FallbackSelectables;

	// Token: 0x04003E65 RID: 15973
	[SerializeField]
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	private InventoryItemSelectableDirectional.SelectableList[] nextPages = new InventoryItemSelectableDirectional.SelectableList[0];

	// Token: 0x04003E66 RID: 15974
	private InventoryAutoNavGroup autoNavGroup;

	// Token: 0x0200199F RID: 6559
	[Serializable]
	public class SelectableList
	{
		// Token: 0x04009672 RID: 38514
		public List<InventoryItemSelectable> Selectables;
	}
}
