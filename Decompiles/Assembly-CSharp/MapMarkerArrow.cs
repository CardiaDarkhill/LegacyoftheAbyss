using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020006CF RID: 1743
public abstract class MapMarkerArrow : MonoBehaviour
{
	// Token: 0x06003EE5 RID: 16101 RVA: 0x00114CD8 File Offset: 0x00112ED8
	private void Awake()
	{
		Transform transform = base.transform;
		this.initialPos = transform.localPosition;
		this.gameMap = base.GetComponentInParent<GameMap>();
		if (this.gameMap)
		{
			this.gameMap.UpdateQuickMapDisplay += this.OnGameMapUpdateQuickMapDisplay;
			this.gameMap.ViewPosUpdated += this.OnGameMapViewPosUpdated;
			this.arrow = Object.Instantiate<GameObject>(this.arrowPrefab, transform);
			this.arrow.transform.Reset();
			this.arrow.SetActive(false);
		}
		else
		{
			this.wideMap = base.GetComponentInParent<InventoryWideMap>();
			if (this.wideMap)
			{
				this.wideMap.PlacedCompassIcon += this.OnWideMapPlacedCompassIcon;
			}
		}
		base.gameObject.SetActive(false);
		if (this.gameMap)
		{
			this.viewportEdge = this.gameMap.ViewportEdge;
		}
	}

	// Token: 0x06003EE6 RID: 16102 RVA: 0x00114DD0 File Offset: 0x00112FD0
	private void OnDestroy()
	{
		if (this.gameMap)
		{
			this.gameMap.UpdateQuickMapDisplay -= this.OnGameMapUpdateQuickMapDisplay;
			this.gameMap.ViewPosUpdated -= this.OnGameMapViewPosUpdated;
		}
		if (this.wideMap)
		{
			this.wideMap.PlacedCompassIcon -= this.OnWideMapPlacedCompassIcon;
		}
	}

	// Token: 0x06003EE7 RID: 16103 RVA: 0x00114E3C File Offset: 0x0011303C
	private void OnWideMapPlacedCompassIcon()
	{
		this.OnGameMapUpdateQuickMapDisplay(false, MapZone.NONE);
	}

	// Token: 0x06003EE8 RID: 16104 RVA: 0x00114E48 File Offset: 0x00113048
	private void OnGameMapUpdateQuickMapDisplay(bool isQuickMap, MapZone currentMapZone)
	{
		if (this.IsActive(isQuickMap, currentMapZone))
		{
			base.gameObject.SetActive(true);
			if (isQuickMap)
			{
				base.transform.SetLocalPosition2D(this.initialPos);
				this.wasOutsideView = false;
				this.arrow.SetActive(false);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		GameMapPinLayout componentInParent = base.GetComponentInParent<GameMapPinLayout>();
		if (componentInParent)
		{
			componentInParent.DoLayout();
		}
	}

	// Token: 0x06003EE9 RID: 16105 RVA: 0x00114EB8 File Offset: 0x001130B8
	private void OnGameMapViewPosUpdated(Vector2 pos)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		Vector3 v = base.transform.parent.TransformPoint(this.initialPos);
		bool flag = !this.viewportEdge.OverlapPoint(v);
		Vector2 vector = flag ? base.transform.parent.InverseTransformPoint(this.viewportEdge.ClosestPoint(v)) : this.initialPos;
		base.transform.SetLocalPosition2D(vector);
		if (flag)
		{
			if (!this.wasOutsideView)
			{
				this.arrow.SetActive(true);
			}
			float rotation = (this.initialPos - vector).DirectionToAngle();
			this.arrow.transform.SetLocalRotation2D(rotation);
		}
		else if (this.wasOutsideView)
		{
			this.arrow.SetActive(false);
		}
		this.wasOutsideView = flag;
	}

	// Token: 0x06003EEA RID: 16106 RVA: 0x00114F9E File Offset: 0x0011319E
	public void SetPosition(Vector2 position)
	{
		this.initialPos = position;
	}

	// Token: 0x06003EEB RID: 16107
	protected abstract bool IsActive(bool isQuickMap, MapZone currentMapZone);

	// Token: 0x0400408B RID: 16523
	[SerializeField]
	private GameObject arrowPrefab;

	// Token: 0x0400408C RID: 16524
	private Vector2 initialPos;

	// Token: 0x0400408D RID: 16525
	private GameObject arrow;

	// Token: 0x0400408E RID: 16526
	private bool wasOutsideView;

	// Token: 0x0400408F RID: 16527
	private GameMap gameMap;

	// Token: 0x04004090 RID: 16528
	private Collider2D viewportEdge;

	// Token: 0x04004091 RID: 16529
	private InventoryWideMap wideMap;
}
