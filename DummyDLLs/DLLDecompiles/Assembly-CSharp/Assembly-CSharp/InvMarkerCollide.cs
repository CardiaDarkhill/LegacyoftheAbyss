using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C1 RID: 1729
public class InvMarkerCollide : MonoBehaviour
{
	// Token: 0x06003EA5 RID: 16037 RVA: 0x00113D45 File Offset: 0x00111F45
	private void Awake()
	{
		this.circle = base.GetComponent<CircleCollider2D>();
	}

	// Token: 0x06003EA6 RID: 16038 RVA: 0x00113D53 File Offset: 0x00111F53
	private void OnDisable()
	{
		this.ResetMarkerSelection();
	}

	// Token: 0x06003EA7 RID: 16039 RVA: 0x00113D5C File Offset: 0x00111F5C
	private void Update()
	{
		if (!PlayerData.instance.isInventoryOpen)
		{
			return;
		}
		Vector2 worldPos = base.transform.TransformPoint(this.circle.offset);
		float radius = this.circle.radius;
		this.ResetMarkerSelection();
		foreach (InvMarker invMarker in InvMarker.GetMarkerList())
		{
			this.previousMarkers.Add(invMarker);
			if (invMarker.IsColliding(worldPos, radius))
			{
				this.markerMenu.AddToCollidingList(invMarker.gameObject);
			}
			else
			{
				this.markerMenu.RemoveFromCollidingList(invMarker.gameObject);
			}
		}
	}

	// Token: 0x06003EA8 RID: 16040 RVA: 0x00113E24 File Offset: 0x00112024
	private void ResetMarkerSelection()
	{
		foreach (InvMarker invMarker in this.previousMarkers)
		{
			if (!InvMarker.GetMarkerList().Contains(invMarker))
			{
				this.markerMenu.RemoveFromCollidingList(invMarker.gameObject);
			}
		}
		this.previousMarkers.Clear();
	}

	// Token: 0x04004049 RID: 16457
	[SerializeField]
	private MapMarkerMenu markerMenu;

	// Token: 0x0400404A RID: 16458
	private CircleCollider2D circle;

	// Token: 0x0400404B RID: 16459
	private readonly List<InvMarker> previousMarkers = new List<InvMarker>();
}
