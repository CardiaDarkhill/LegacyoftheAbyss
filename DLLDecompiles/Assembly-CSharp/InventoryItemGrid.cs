using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x0200068D RID: 1677
public class InventoryItemGrid : InventoryItemSelectable, IInventorySelectionParent
{
	// Token: 0x170006C5 RID: 1733
	// (get) Token: 0x06003BD4 RID: 15316 RVA: 0x0010701F File Offset: 0x0010521F
	public InventoryItemGrid.ListSetupTypes ListSetupType
	{
		get
		{
			return this.listSetupType;
		}
	}

	// Token: 0x170006C6 RID: 1734
	// (get) Token: 0x06003BD5 RID: 15317 RVA: 0x00107027 File Offset: 0x00105227
	public override string DisplayName
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170006C7 RID: 1735
	// (get) Token: 0x06003BD6 RID: 15318 RVA: 0x0010702E File Offset: 0x0010522E
	public override string Description
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x06003BD7 RID: 15319 RVA: 0x00107038 File Offset: 0x00105238
	protected void Awake()
	{
		this.itemManager = base.GetComponentInParent<InventoryItemManager>();
		for (int i = 0; i < this.selectables.Length; i++)
		{
			InventoryItemGrid.SelectableList selectableList = this.selectables[i];
			selectableList.Selectables = (from o in selectableList.Selectables
			where o != null
			select o).ToArray<InventoryItemSelectable>();
		}
	}

	// Token: 0x06003BD8 RID: 15320 RVA: 0x001070A0 File Offset: 0x001052A0
	protected void Start()
	{
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			componentInParent.OnPaneStart += this.Setup;
		}
		this.OnValidate();
		List<InventoryItemGrid.GridSection> list = this.collections;
		if (list != null && list.Count > 0 && this.collections[0].Items.Count > 0)
		{
			this.ScrollEventHandler(this.collections[0].Items[0]);
		}
	}

	// Token: 0x06003BD9 RID: 15321 RVA: 0x0010711D File Offset: 0x0010531D
	protected void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<InventoryItemGrid.SelectableList>(ref this.selectables, typeof(InventoryItemManager.SelectionDirection));
		ArrayForEnumAttribute.EnsureArraySize<InventoryItemGrid.SelectableList>(ref this.nextPages, typeof(InventoryItemManager.SelectionDirection));
		this.Setup();
	}

	// Token: 0x06003BDA RID: 15322 RVA: 0x00107150 File Offset: 0x00105350
	[ContextMenu("Refresh Preview")]
	public void Setup()
	{
		if (this.listSetupType != InventoryItemGrid.ListSetupTypes.Auto)
		{
			return;
		}
		List<InventoryItemSelectableDirectional> items = (from item in new List<InventoryItemSelectableDirectional>(base.GetComponentsInChildren<InventoryItemSelectableDirectional>())
		where item.gameObject.activeSelf
		select item).ToList<InventoryItemSelectableDirectional>();
		List<InventoryItemGrid.GridSection> newCollections = new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Header = null,
				Items = items
			}
		};
		this.Setup(newCollections);
	}

	// Token: 0x06003BDB RID: 15323 RVA: 0x001071C4 File Offset: 0x001053C4
	public void Setup(List<InventoryItemGrid.GridSection> newCollections)
	{
		for (int i = 0; i < newCollections.Count; i++)
		{
			InventoryItemGrid.GridSection gridSection = newCollections[i];
			if (gridSection.Header)
			{
				bool flag = i > 0 && newCollections[i - 1].Items.Count > 0;
				bool flag2 = false;
				using (List<InventoryItemSelectableDirectional>.Enumerator enumerator = gridSection.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.gameObject.activeSelf)
						{
							flag2 = true;
							break;
						}
					}
				}
				bool active = flag2 && (!gridSection.HideHeaderIfNoneBefore || flag);
				gridSection.Header.gameObject.SetActive(active);
			}
		}
		this.SetupScrollEvents(false);
		newCollections = (this.collections = (from collection in newCollections
		where collection.Items.Count > 0
		select collection).ToList<InventoryItemGrid.GridSection>());
		this.SetupScrollEvents(true);
		for (int j = 0; j < newCollections.Count; j++)
		{
			InventoryItemGrid.GridSection gridSection2 = newCollections[j];
			InventoryItemGrid.GridSection gridSection3 = (j > 0) ? newCollections[j - 1] : null;
			InventoryItemGrid.GridSection gridSection4 = (j < newCollections.Count - 1) ? newCollections[j + 1] : null;
			List<InventoryItemSelectableDirectional> upOverrides = null;
			List<InventoryItemSelectableDirectional> downOverrides = null;
			if (gridSection3 != null)
			{
				int num = gridSection3.Items.Count % this.RowSplit;
				if (num == 0)
				{
					num = this.RowSplit;
				}
				upOverrides = gridSection3.Items.Skip(gridSection3.Items.Count - num).ToList<InventoryItemSelectableDirectional>();
			}
			if (gridSection4 != null)
			{
				downOverrides = gridSection4.Items.Take(this.RowSplit).ToList<InventoryItemSelectableDirectional>();
			}
			this.LinkGridSelectables(gridSection2.Items, downOverrides, upOverrides);
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional = (gridSection3 != null) ? gridSection3.Items.LastOrDefault<InventoryItemSelectableDirectional>() : null;
			float num2 = (inventoryItemSelectableDirectional != null) ? (inventoryItemSelectableDirectional.transform.localPosition.y - this.GridOffset.y + this.ItemOffset.y) : 0f;
			this.PositionGridItems(gridSection2.Items, num2 - this.SectionPadding);
			if (newCollections[j].Header)
			{
				newCollections[j].Header.SetLocalPositionY(newCollections[j].Items[0].transform.localPosition.y + this.SectionHeaderHeight);
			}
			for (int k = 0; k < gridSection2.Items.Count; k++)
			{
				InventoryItemSelectableDirectional inventoryItemSelectableDirectional2 = gridSection2.Items[k];
				inventoryItemSelectableDirectional2.Grid = this;
				inventoryItemSelectableDirectional2.GridSectionIndex = j;
				inventoryItemSelectableDirectional2.GridItemIndex = k;
			}
		}
		if (this.scrollView)
		{
			this.scrollView.FullUpdate();
			if (!this.hasDoneInitialScroll && !newCollections.IsNullOrEmpty<InventoryItemGrid.GridSection>() && !newCollections[0].Items.IsNullOrEmpty<InventoryItemSelectableDirectional>())
			{
				if (this.queuedScroll != null)
				{
					this.ScrollTo(this.queuedScroll, true);
				}
				else
				{
					this.ScrollTo(newCollections[0].Items[0].transform.position, true);
				}
				this.hasDoneInitialScroll = true;
			}
			else if (this.queuedScroll != null)
			{
				this.ScrollTo(this.queuedScroll, true);
			}
		}
		if (this.attachToBottom)
		{
			this.attachToBottom.SetPosition2D(base.transform.position);
			if (newCollections.Count == 0)
			{
				return;
			}
			List<InventoryItemGrid.GridSection> list = newCollections;
			InventoryItemGrid.GridSection gridSection5 = list[list.Count - 1];
			if (gridSection5.Items.Count == 0)
			{
				return;
			}
			List<InventoryItemSelectableDirectional> items = gridSection5.Items;
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional3 = items[items.Count - 1];
			Vector2 a = new Vector2(base.transform.position.x, inventoryItemSelectableDirectional3.transform.position.y);
			this.attachToBottom.SetPosition2D(a + this.bottomOffset);
		}
	}

	// Token: 0x06003BDC RID: 15324 RVA: 0x001075EC File Offset: 0x001057EC
	private void LinkGridSelectables(List<InventoryItemSelectableDirectional> childItems, List<InventoryItemSelectableDirectional> downOverrides, List<InventoryItemSelectableDirectional> upOverrides)
	{
		int rowSplit = this.RowSplit;
		int num = Mathf.CeilToInt((float)childItems.Count / (float)this.RowSplit) - 1;
		for (int i = 0; i < childItems.Count; i++)
		{
			int num2 = i % rowSplit;
			int num3 = i / rowSplit;
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional = childItems[i];
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional2 = (upOverrides != null && upOverrides.Count > 0) ? upOverrides[Mathf.Min(num2, upOverrides.Count - 1)] : null;
			InventoryItemSelectableDirectional inventoryItemSelectableDirectional3 = (downOverrides != null && downOverrides.Count > 0) ? downOverrides[Mathf.Min(num2, downOverrides.Count - 1)] : null;
			if (!(inventoryItemSelectableDirectional == null))
			{
				inventoryItemSelectableDirectional.Selectables[2] = childItems.GetBy2DIndexes(rowSplit, num2 - 1, num3, null);
				inventoryItemSelectableDirectional.Selectables[3] = childItems.GetBy2DIndexes(rowSplit, num2 + 1, num3, null);
				inventoryItemSelectableDirectional.Selectables[0] = childItems.GetBy2DIndexes(rowSplit, num2, num3 - 1, inventoryItemSelectableDirectional2 ? inventoryItemSelectableDirectional2 : null);
				inventoryItemSelectableDirectional.Selectables[1] = childItems.GetBy2DIndexes(rowSplit, num2, num3 + 1, null);
				if (inventoryItemSelectableDirectional.Selectables[1] == null)
				{
					if (num3 < num)
					{
						inventoryItemSelectableDirectional.Selectables[1] = childItems[childItems.Count - 1];
					}
					else
					{
						inventoryItemSelectableDirectional.Selectables[1] = (inventoryItemSelectableDirectional3 ? inventoryItemSelectableDirectional3 : null);
					}
				}
				if (inventoryItemSelectableDirectional.Selectables[3] == null && num2 < rowSplit - 1 && num3 > 0)
				{
					inventoryItemSelectableDirectional.Selectables[3] = childItems.GetBy2DIndexes(rowSplit, Mathf.Min(num2 + 1, rowSplit - 1), num3 - 1, null);
				}
			}
		}
	}

	// Token: 0x06003BDD RID: 15325 RVA: 0x00107780 File Offset: 0x00105980
	private void PositionGridItems(List<InventoryItemSelectableDirectional> childItems, float yOffset)
	{
		int rowSplit = this.RowSplit;
		for (int i = 0; i < childItems.Count; i++)
		{
			int num = i % rowSplit;
			int num2 = i / rowSplit;
			childItems[i].transform.SetLocalPosition2D(this.GridOffset + new Vector2(this.ItemOffset.x * (float)num, this.ItemOffset.y * (float)num2 + yOffset));
		}
	}

	// Token: 0x06003BDE RID: 15326 RVA: 0x001077EC File Offset: 0x001059EC
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (this.collections == null || this.collections.Count == 0)
		{
			return null;
		}
		InventoryItemGrid.GridSection gridSection = this.collections[0];
		if (gridSection == null || gridSection.Items.Count <= 0)
		{
			return null;
		}
		if (direction == null)
		{
			return gridSection.Items[0].Get(direction);
		}
		if (this.itemManager && this.itemManager.CurrentSelected)
		{
			IEnumerable<InventoryItemSelectableDirectional> items = this.collections.SelectMany((InventoryItemGrid.GridSection col) => col.Items);
			InventoryItemSelectableDirectional closestOnAxis = InventoryItemNavigationHelper.GetClosestOnAxis<InventoryItemSelectableDirectional>(direction.Value, this.itemManager.CurrentSelected, items);
			if (closestOnAxis)
			{
				return closestOnAxis.Get(direction);
			}
		}
		InventoryItemManager.SelectionDirection value = direction.Value;
		if (value != InventoryItemManager.SelectionDirection.Left)
		{
			return gridSection.Items[0].Get(direction);
		}
		return gridSection.Items[Mathf.Min(this.collections[0].Items.Count, this.RowSplit) - 1].Get(direction);
	}

	// Token: 0x06003BDF RID: 15327 RVA: 0x00107920 File Offset: 0x00105B20
	public override InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectableDirectional inventoryItemSelectableDirectional = currentSelected as InventoryItemSelectableDirectional;
		if (!inventoryItemSelectableDirectional)
		{
			return base.GetNextSelectablePage(currentSelected, direction);
		}
		for (int i = 0; i < this.collections.Count; i++)
		{
			InventoryItemGrid.GridSection gridSection = this.collections[i];
			int num = gridSection.Items.IndexOf(inventoryItemSelectableDirectional);
			if (num >= 0)
			{
				if (direction != InventoryItemManager.SelectionDirection.Up)
				{
					if (direction == InventoryItemManager.SelectionDirection.Down)
					{
						if (i < this.collections.Count - 1)
						{
							InventoryItemGrid.GridSection gridSection2 = this.collections[i + 1];
							if (gridSection2.Items.Count > 0)
							{
								return gridSection2.Items[0];
							}
						}
						if (gridSection.Items.Count > 0)
						{
							List<InventoryItemSelectableDirectional> items = gridSection.Items;
							return items[items.Count - 1];
						}
					}
				}
				else
				{
					if (num > 0)
					{
						return gridSection.Items[0];
					}
					if (i > 0)
					{
						InventoryItemGrid.GridSection gridSection3 = this.collections[i - 1];
						if (gridSection3.Items.Count > 0)
						{
							return gridSection3.Items[0];
						}
					}
				}
			}
		}
		InventoryItemGrid.SelectableList selectableList = this.nextPages[(int)direction];
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

	// Token: 0x06003BE0 RID: 15328 RVA: 0x00107A7C File Offset: 0x00105C7C
	public List<T> GetListItems<T>(Func<T, bool> evaluation = null) where T : InventoryItemSelectable
	{
		if (evaluation == null)
		{
			evaluation = ((T _) => true);
		}
		return (from T selectable in this.collections.SelectMany((InventoryItemGrid.GridSection section) => section.Items)
		where evaluation(selectable)
		select selectable).ToList<T>();
	}

	// Token: 0x06003BE1 RID: 15329 RVA: 0x00107B08 File Offset: 0x00105D08
	private void SetupScrollEvents(bool subscribe)
	{
		if (this.collections == null || this.collections.Count == 0)
		{
			return;
		}
		foreach (InventoryItemGrid.GridSection gridSection in this.collections)
		{
			foreach (InventoryItemSelectableDirectional inventoryItemSelectableDirectional in gridSection.Items)
			{
				if (subscribe)
				{
					inventoryItemSelectableDirectional.OnSelected += this.ScrollEventHandler;
				}
				else
				{
					inventoryItemSelectableDirectional.OnSelected -= this.ScrollEventHandler;
				}
			}
		}
	}

	// Token: 0x06003BE2 RID: 15330 RVA: 0x00107BCC File Offset: 0x00105DCC
	private void ScrollEventHandler(InventoryItemSelectable item)
	{
		this.ScrollTo(item, false);
	}

	// Token: 0x06003BE3 RID: 15331 RVA: 0x00107BD8 File Offset: 0x00105DD8
	public void ScrollTo(InventoryItemSelectable item, bool isInstant = false)
	{
		if (item == null)
		{
			return;
		}
		if (!this.hasDoneInitialScroll || (isInstant && !base.gameObject.activeInHierarchy))
		{
			this.queuedScroll = item;
			return;
		}
		this.ScrollTo(item.transform.position, isInstant);
	}

	// Token: 0x06003BE4 RID: 15332 RVA: 0x00107C28 File Offset: 0x00105E28
	private void ScrollTo(Vector2 worldPos, bool isInstant)
	{
		if (!this.scrollView)
		{
			return;
		}
		Vector2 localPosition = this.scrollView.transform.InverseTransformPoint(worldPos);
		this.scrollView.ScrollTo(localPosition, isInstant);
		this.queuedScroll = null;
	}

	// Token: 0x06003BE5 RID: 15333 RVA: 0x00107C73 File Offset: 0x00105E73
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return this;
	}

	// Token: 0x06003BE6 RID: 15334 RVA: 0x00107C78 File Offset: 0x00105E78
	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction)
	{
		if (direction == null)
		{
			return null;
		}
		Vector2 b = source.transform.position;
		InventoryItemManager.SelectionDirection value = direction.Value;
		Vector2 vector;
		if (value > InventoryItemManager.SelectionDirection.Down)
		{
			if (value - InventoryItemManager.SelectionDirection.Left > 1)
			{
				throw new ArgumentOutOfRangeException();
			}
			vector = new Vector2(1f, 2f);
		}
		else
		{
			vector = new Vector2(2f, 1f);
		}
		Vector2 b2 = vector;
		InventoryItemSelectable result = null;
		float num = float.MaxValue;
		foreach (InventoryItemSelectable inventoryItemSelectable in this.selectables[(int)direction.Value].Selectables)
		{
			Vector2 a = inventoryItemSelectable.transform.position - b;
			float magnitude = (a * b2).magnitude;
			if (magnitude <= num)
			{
				num = magnitude;
				result = inventoryItemSelectable;
			}
		}
		return result;
	}

	// Token: 0x06003BE7 RID: 15335 RVA: 0x00107D58 File Offset: 0x00105F58
	public InventoryItemSelectable GetItemOrFallback(int sectionIndex, int itemIndex)
	{
		if (this.collections.Count == 0)
		{
			return null;
		}
		bool flag = false;
		if (sectionIndex >= this.collections.Count)
		{
			sectionIndex = this.collections.Count - 1;
			flag = true;
		}
		InventoryItemGrid.GridSection gridSection = this.collections[sectionIndex];
		if (gridSection.Items.Count == 0)
		{
			return null;
		}
		if (flag || itemIndex >= gridSection.Items.Count)
		{
			itemIndex = gridSection.Items.Count - 1;
		}
		return gridSection.Items[itemIndex];
	}

	// Token: 0x06003BE8 RID: 15336 RVA: 0x00107DE0 File Offset: 0x00105FE0
	public InventoryItemSelectable GetFirst()
	{
		if (this.collections.Count == 0)
		{
			return null;
		}
		InventoryItemGrid.GridSection gridSection = this.collections[0];
		if (gridSection.Items.Count == 0)
		{
			return null;
		}
		return gridSection.Items[0];
	}

	// Token: 0x06003BE9 RID: 15337 RVA: 0x00107E24 File Offset: 0x00106024
	public InventoryItemSelectable GetLast()
	{
		if (this.collections.Count == 0)
		{
			return null;
		}
		List<InventoryItemGrid.GridSection> list = this.collections;
		InventoryItemGrid.GridSection gridSection = list[list.Count - 1];
		if (gridSection.Items.Count == 0)
		{
			return null;
		}
		List<InventoryItemSelectableDirectional> items = gridSection.Items;
		return items[items.Count - 1];
	}

	// Token: 0x06003BEA RID: 15338 RVA: 0x00107E78 File Offset: 0x00106078
	public static void LinkVertical(InventoryItemSelectableDirectional top, InventoryItemGrid bottom)
	{
		if (top)
		{
			top.Selectables[1] = bottom;
		}
		if (bottom)
		{
			bottom.selectables[0] = new InventoryItemGrid.SelectableList
			{
				Selectables = new InventoryItemSelectable[]
				{
					top
				}
			};
		}
	}

	// Token: 0x04003DFE RID: 15870
	[SerializeField]
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	private InventoryItemGrid.SelectableList[] selectables;

	// Token: 0x04003DFF RID: 15871
	[SerializeField]
	[ArrayForEnum(typeof(InventoryItemManager.SelectionDirection))]
	private InventoryItemGrid.SelectableList[] nextPages;

	// Token: 0x04003E00 RID: 15872
	[Space]
	public int RowSplit = 4;

	// Token: 0x04003E01 RID: 15873
	public Vector2 GridOffset;

	// Token: 0x04003E02 RID: 15874
	public Vector2 ItemOffset;

	// Token: 0x04003E03 RID: 15875
	[SerializeField]
	private Transform attachToBottom;

	// Token: 0x04003E04 RID: 15876
	[SerializeField]
	private Vector2 bottomOffset;

	// Token: 0x04003E05 RID: 15877
	[Space]
	public float SectionPadding = 2.5f;

	// Token: 0x04003E06 RID: 15878
	public float SectionHeaderHeight = 1f;

	// Token: 0x04003E07 RID: 15879
	[Space]
	[SerializeField]
	private InventoryItemGrid.ListSetupTypes listSetupType;

	// Token: 0x04003E08 RID: 15880
	[SerializeField]
	private ScrollView scrollView;

	// Token: 0x04003E09 RID: 15881
	private List<InventoryItemGrid.GridSection> collections;

	// Token: 0x04003E0A RID: 15882
	private bool hasDoneInitialScroll;

	// Token: 0x04003E0B RID: 15883
	private InventoryItemManager itemManager;

	// Token: 0x04003E0C RID: 15884
	private InventoryItemSelectable queuedScroll;

	// Token: 0x02001990 RID: 6544
	[Serializable]
	private class SelectableList
	{
		// Token: 0x04009646 RID: 38470
		public InventoryItemSelectable[] Selectables;
	}

	// Token: 0x02001991 RID: 6545
	public class GridSection
	{
		// Token: 0x04009647 RID: 38471
		public Transform Header;

		// Token: 0x04009648 RID: 38472
		public bool HideHeaderIfNoneBefore;

		// Token: 0x04009649 RID: 38473
		public List<InventoryItemSelectableDirectional> Items = new List<InventoryItemSelectableDirectional>();
	}

	// Token: 0x02001992 RID: 6546
	public enum ListSetupTypes
	{
		// Token: 0x0400964B RID: 38475
		Auto,
		// Token: 0x0400964C RID: 38476
		[UsedImplicitly]
		Custom
	}
}
