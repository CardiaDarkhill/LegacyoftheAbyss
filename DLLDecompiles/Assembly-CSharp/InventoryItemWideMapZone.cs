using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006AA RID: 1706
public class InventoryItemWideMapZone : InventoryItemSelectableDirectional
{
	// Token: 0x17000703 RID: 1795
	// (get) Token: 0x06003D3B RID: 15675 RVA: 0x0010D72F File Offset: 0x0010B92F
	public MapZone ZoomToZone
	{
		get
		{
			return this.zoomToZone;
		}
	}

	// Token: 0x17000704 RID: 1796
	// (get) Token: 0x06003D3C RID: 15676 RVA: 0x0010D737 File Offset: 0x0010B937
	public override bool ShowCursor
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000705 RID: 1797
	// (get) Token: 0x06003D3D RID: 15677 RVA: 0x0010D73C File Offset: 0x0010B93C
	public bool IsUnlocked
	{
		get
		{
			GameMap gameMap = GameManager.instance.gameMap;
			if (CollectableItemManager.IsInHiddenMode())
			{
				if (!gameMap.HasAnyMapForZone(MapZone.THE_SLAB))
				{
					return false;
				}
				if (!this.mapZones.IsBitSet(7))
				{
					return false;
				}
			}
			if (gameMap.IsLostInAbyssPreMap())
			{
				return false;
			}
			if (gameMap.IsLostInAbyssPostMap() && !this.mapZones.IsBitSet(37))
			{
				return false;
			}
			foreach (MapZone mapZone in this.EnumerateMapZones())
			{
				if (gameMap.HasAnyMapForZone(mapZone))
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x17000706 RID: 1798
	// (get) Token: 0x06003D3E RID: 15678 RVA: 0x0010D7E4 File Offset: 0x0010B9E4
	protected override bool IsAutoNavSelectable
	{
		get
		{
			return this.IsUnlocked;
		}
	}

	// Token: 0x06003D3F RID: 15679 RVA: 0x0010D7EC File Offset: 0x0010B9EC
	private void Reset()
	{
		this.sprite = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06003D40 RID: 15680 RVA: 0x0010D7FA File Offset: 0x0010B9FA
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(base.NavigationOffset, 0.2f);
	}

	// Token: 0x06003D41 RID: 15681 RVA: 0x0010D824 File Offset: 0x0010BA24
	protected override void Awake()
	{
		base.Awake();
		this.manager = base.GetComponentInParent<InventoryMapManager>();
		if (this.sprite)
		{
			this.initialColor = this.sprite.color;
			this.initialColor.a = 1f;
		}
		if (this.labelText)
		{
			this.initialLabelColor = this.labelText.color;
			this.initialLabelColor.a = 1f;
		}
		this.pane = base.GetComponentInParent<InventoryPane>();
		if (this.pane)
		{
			this.pane.OnPaneStart += this.EvaluateUnlocked;
		}
	}

	// Token: 0x06003D42 RID: 15682 RVA: 0x0010D8CF File Offset: 0x0010BACF
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		this.isSelected = true;
		this.UpdateColor();
	}

	// Token: 0x06003D43 RID: 15683 RVA: 0x0010D8E5 File Offset: 0x0010BAE5
	public override void Deselect()
	{
		base.Deselect();
		this.isSelected = false;
		this.UpdateColor();
	}

	// Token: 0x06003D44 RID: 15684 RVA: 0x0010D8FC File Offset: 0x0010BAFC
	public override bool Submit()
	{
		MapZone currentMapZone = this.manager.GetCurrentMapZone();
		MapZone mapZone = this.mapZones.IsBitSet((int)currentMapZone) ? currentMapZone : this.zoomToZone;
		this.manager.ZoomIn(mapZone, true);
		return true;
	}

	// Token: 0x06003D45 RID: 15685 RVA: 0x0010D93C File Offset: 0x0010BB3C
	private void UpdateColor()
	{
		if (this.sprite)
		{
			this.sprite.color = (this.isSelected ? this.initialColor : this.initialColor.MultiplyElements(InventoryItemWideMapZone._deselectedMultiplyColor));
		}
		if (this.labelText)
		{
			this.labelText.color = (this.isSelected ? this.initialLabelColor : this.initialLabelColor.MultiplyElements(InventoryItemWideMapZone._deselectedMultiplyColor));
		}
	}

	// Token: 0x06003D46 RID: 15686 RVA: 0x0010D9B9 File Offset: 0x0010BBB9
	private void EvaluateUnlocked()
	{
		if (this.IsUnlocked)
		{
			if (base.gameObject.activeSelf)
			{
				base.EvaluateAutoNav();
			}
			else
			{
				base.gameObject.SetActive(true);
			}
			this.UpdateColor();
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x0010D9F8 File Offset: 0x0010BBF8
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectable nextSelectable = base.GetNextSelectable(direction, false);
		if (!nextSelectable)
		{
			return null;
		}
		InventoryItemWideMapZone inventoryItemWideMapZone = nextSelectable as InventoryItemWideMapZone;
		if (inventoryItemWideMapZone == null || inventoryItemWideMapZone.gameObject.activeSelf)
		{
			return nextSelectable;
		}
		return base.GetSelectableFromAutoNavGroup<InventoryItemWideMapZone>(direction, (InventoryItemWideMapZone zone) => zone.gameObject.activeSelf);
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x0010DA60 File Offset: 0x0010BC60
	public Vector2 GetClosestNodePosLocalBounds(Vector2 localBoundsPos)
	{
		Bounds bounds = this.sprite.bounds;
		Vector3 vector = base.transform.InverseTransformPoint(bounds.min);
		Vector3 vector2 = base.transform.InverseTransformPoint(bounds.max);
		Vector2 b = new Vector2(Mathf.Lerp(vector.x, vector2.x, localBoundsPos.x), Mathf.Lerp(vector.y, vector2.y, localBoundsPos.y));
		Vector2 result = Vector2.zero;
		float num = float.MaxValue;
		foreach (Vector2 vector3 in this.compassNodes)
		{
			float num2 = Vector2.Distance(vector3, b);
			if (num2 <= num)
			{
				result = vector3;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x06003D49 RID: 15689 RVA: 0x0010DB22 File Offset: 0x0010BD22
	public IEnumerable<MapZone> EnumerateMapZones()
	{
		int num;
		for (int i = 0; i < 64; i = num + 1)
		{
			if (this.mapZones.IsBitSet(i))
			{
				yield return (MapZone)i;
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x04003EF7 RID: 16119
	private static readonly Color _deselectedMultiplyColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	// Token: 0x04003EF8 RID: 16120
	[SerializeField]
	private SpriteRenderer sprite;

	// Token: 0x04003EF9 RID: 16121
	[SerializeField]
	[EnumPickerBitmask(typeof(MapZone))]
	private long mapZones;

	// Token: 0x04003EFA RID: 16122
	[SerializeField]
	[FormerlySerializedAs("mapZone")]
	private MapZone zoomToZone;

	// Token: 0x04003EFB RID: 16123
	[SerializeField]
	private TMP_Text labelText;

	// Token: 0x04003EFC RID: 16124
	[SerializeField]
	private Vector2[] compassNodes;

	// Token: 0x04003EFD RID: 16125
	private Color initialColor;

	// Token: 0x04003EFE RID: 16126
	private Color initialLabelColor;

	// Token: 0x04003EFF RID: 16127
	private bool isSelected;

	// Token: 0x04003F00 RID: 16128
	private InventoryMapManager manager;

	// Token: 0x04003F01 RID: 16129
	private InventoryPane pane;
}
