using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C0 RID: 1728
public class InvMarker : MonoBehaviour
{
	// Token: 0x1700073A RID: 1850
	// (get) Token: 0x06003E9A RID: 16026 RVA: 0x00113C94 File Offset: 0x00111E94
	// (set) Token: 0x06003E9B RID: 16027 RVA: 0x00113C9C File Offset: 0x00111E9C
	public MapMarkerMenu.MarkerTypes Colour { get; set; }

	// Token: 0x1700073B RID: 1851
	// (get) Token: 0x06003E9C RID: 16028 RVA: 0x00113CA5 File Offset: 0x00111EA5
	// (set) Token: 0x06003E9D RID: 16029 RVA: 0x00113CAD File Offset: 0x00111EAD
	public int Index { get; set; }

	// Token: 0x06003E9E RID: 16030 RVA: 0x00113CB6 File Offset: 0x00111EB6
	private void Awake()
	{
		this.circle = base.GetComponent<CircleCollider2D>();
	}

	// Token: 0x06003E9F RID: 16031 RVA: 0x00113CC4 File Offset: 0x00111EC4
	private void OnEnable()
	{
		InvMarker._activeMarkers.Add(this);
	}

	// Token: 0x06003EA0 RID: 16032 RVA: 0x00113CD1 File Offset: 0x00111ED1
	private void OnDisable()
	{
		InvMarker._activeMarkers.Remove(this);
	}

	// Token: 0x06003EA1 RID: 16033 RVA: 0x00113CDF File Offset: 0x00111EDF
	public static List<InvMarker> GetMarkerList()
	{
		return InvMarker._activeMarkers;
	}

	// Token: 0x06003EA2 RID: 16034 RVA: 0x00113CE8 File Offset: 0x00111EE8
	public bool IsColliding(Vector2 worldPos, float radius)
	{
		Vector2 b = base.transform.TransformPoint(this.circle.offset);
		float num = Vector2.Distance(worldPos, b);
		float num2 = radius + this.circle.radius;
		return num <= num2;
	}

	// Token: 0x04004045 RID: 16453
	private CircleCollider2D circle;

	// Token: 0x04004046 RID: 16454
	private static readonly List<InvMarker> _activeMarkers = new List<InvMarker>();
}
