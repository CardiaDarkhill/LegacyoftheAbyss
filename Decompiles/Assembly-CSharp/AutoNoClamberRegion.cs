using System;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class AutoNoClamberRegion : NoClamberRegion
{
	// Token: 0x060006B1 RID: 1713 RVA: 0x000220C8 File Offset: 0x000202C8
	private void FixedUpdate()
	{
		Vector2 a = base.transform.position;
		this.isMoving = ((a - this.previousPosition).magnitude > Mathf.Epsilon);
		if (this.isMoving && this.isInsideState)
		{
			if (!this.isInsideRegion)
			{
				NoClamberRegion.InsideRegions.Add(this);
				this.isInsideRegion = true;
			}
		}
		else if (this.isInsideRegion)
		{
			NoClamberRegion.InsideRegions.Remove(this);
			this.isInsideRegion = false;
		}
		this.previousPosition = a;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00022159 File Offset: 0x00020359
	protected override void OnInsideStateChanged(bool isInside)
	{
		this.isInsideState = isInside;
	}

	// Token: 0x04000690 RID: 1680
	private Vector2 previousPosition;

	// Token: 0x04000691 RID: 1681
	private bool isMoving;

	// Token: 0x04000692 RID: 1682
	private bool isInsideState;

	// Token: 0x04000693 RID: 1683
	private bool isInsideRegion;
}
