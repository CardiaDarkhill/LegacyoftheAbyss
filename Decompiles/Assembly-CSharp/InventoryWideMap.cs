using System;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006BB RID: 1723
public class InventoryWideMap : MonoBehaviour
{
	// Token: 0x140000DC RID: 220
	// (add) Token: 0x06003E85 RID: 16005 RVA: 0x00113770 File Offset: 0x00111970
	// (remove) Token: 0x06003E86 RID: 16006 RVA: 0x001137A8 File Offset: 0x001119A8
	public event Action PlacedCompassIcon;

	// Token: 0x17000737 RID: 1847
	// (get) Token: 0x06003E87 RID: 16007 RVA: 0x001137DD File Offset: 0x001119DD
	public NestedFadeGroupBase FadeGroup
	{
		get
		{
			return this.fadeGroup;
		}
	}

	// Token: 0x17000738 RID: 1848
	// (get) Token: 0x06003E88 RID: 16008 RVA: 0x001137E8 File Offset: 0x001119E8
	public InventoryItemWideMapZone[] DefaultSelectables
	{
		get
		{
			InventoryItemWideMapZone[] result;
			if ((result = this.selectables) == null)
			{
				result = (this.selectables = base.GetComponentsInChildren<InventoryItemWideMapZone>(true));
			}
			return result;
		}
	}

	// Token: 0x17000739 RID: 1849
	// (get) Token: 0x06003E89 RID: 16009 RVA: 0x00113810 File Offset: 0x00111A10
	public Vector2 PositionOffset
	{
		get
		{
			if (GameManager.instance.gameMap.IsLostInAbyssPostMap())
			{
				return this.soloAbyssPosition;
			}
			if (CollectableItemManager.IsInHiddenMode())
			{
				return this.soloSlabPosition;
			}
			foreach (InventoryWideMap.ConditionalPosition conditionalPosition in this.positionsOrdered)
			{
				InventoryItemWideMapZone[] zoneConditions = conditionalPosition.ZoneConditions;
				bool flag = false;
				foreach (InventoryItemWideMapZone inventoryItemWideMapZone in zoneConditions)
				{
					if (inventoryItemWideMapZone && inventoryItemWideMapZone.IsUnlocked)
					{
						flag = true;
						break;
					}
				}
				if (zoneConditions.Length == 0 || flag)
				{
					return conditionalPosition.Position;
				}
			}
			return Vector2.zero;
		}
	}

	// Token: 0x06003E8A RID: 16010 RVA: 0x001138B0 File Offset: 0x00111AB0
	public void UpdatePositions()
	{
		if (!InventoryWideMap.IsInvalid(this.PositionOffset))
		{
			base.transform.SetLocalPosition2D(this.PositionOffset);
		}
		Action placedCompassIcon = this.PlacedCompassIcon;
		if (placedCompassIcon != null)
		{
			placedCompassIcon();
		}
		GameManager instance = GameManager.instance;
		if (!instance.gameMap)
		{
			return;
		}
		instance.gameMap.UpdateCurrentScene();
		MapZone currentMapZone;
		Vector2 compassPositionLocalBounds = instance.gameMap.GetCompassPositionLocalBounds(out currentMapZone);
		ToolItem compassTool = Gameplay.CompassTool;
		this.PositionIcon(this.compassIcon, compassPositionLocalBounds, compassTool && compassTool.IsEquipped && !instance.gameMap.IsLostInAbyssPreMap(), currentMapZone);
		MapZone currentMapZone2;
		Vector2 corpsePositionLocalBounds = instance.gameMap.GetCorpsePositionLocalBounds(out currentMapZone2);
		PlayerData instance2 = PlayerData.instance;
		this.PositionIcon(this.corpseIcon, corpsePositionLocalBounds, !string.IsNullOrEmpty(instance2.HeroCorpseScene), currentMapZone2);
	}

	// Token: 0x06003E8B RID: 16011 RVA: 0x00113983 File Offset: 0x00111B83
	private static bool IsInvalid(Vector2 vector2)
	{
		return float.IsNaN(vector2.x) || float.IsInfinity(vector2.y);
	}

	// Token: 0x06003E8C RID: 16012 RVA: 0x001139A0 File Offset: 0x00111BA0
	private void PositionIcon(Transform icon, Vector2 mapBoundsPos, bool isActive, MapZone currentMapZone)
	{
		if (!icon)
		{
			return;
		}
		if (!isActive || InventoryWideMap.IsInvalid(mapBoundsPos))
		{
			icon.gameObject.SetActive(false);
			return;
		}
		icon.gameObject.SetActive(true);
		InventoryItemWideMapZone inventoryItemWideMapZone = null;
		foreach (InventoryItemWideMapZone inventoryItemWideMapZone2 in this.DefaultSelectables)
		{
			if (inventoryItemWideMapZone2.EnumerateMapZones().Contains(currentMapZone))
			{
				inventoryItemWideMapZone = inventoryItemWideMapZone2;
				break;
			}
		}
		if (inventoryItemWideMapZone == null)
		{
			return;
		}
		Vector2 closestNodePosLocalBounds = inventoryItemWideMapZone.GetClosestNodePosLocalBounds(mapBoundsPos);
		icon.SetPosition2D(inventoryItemWideMapZone.transform.TransformPoint(closestNodePosLocalBounds));
	}

	// Token: 0x0400402E RID: 16430
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x0400402F RID: 16431
	[SerializeField]
	private Transform compassIcon;

	// Token: 0x04004030 RID: 16432
	[SerializeField]
	private Transform corpseIcon;

	// Token: 0x04004031 RID: 16433
	[Space]
	[SerializeField]
	private InventoryWideMap.ConditionalPosition[] positionsOrdered;

	// Token: 0x04004032 RID: 16434
	[SerializeField]
	private Vector2 soloSlabPosition;

	// Token: 0x04004033 RID: 16435
	[SerializeField]
	private Vector2 soloAbyssPosition;

	// Token: 0x04004034 RID: 16436
	private InventoryItemWideMapZone[] selectables;

	// Token: 0x020019C9 RID: 6601
	[Serializable]
	private class ConditionalPosition
	{
		// Token: 0x04009730 RID: 38704
		public Vector2 Position;

		// Token: 0x04009731 RID: 38705
		public InventoryItemWideMapZone[] ZoneConditions;
	}
}
