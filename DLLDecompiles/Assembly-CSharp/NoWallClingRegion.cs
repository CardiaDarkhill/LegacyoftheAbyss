using System;
using System.Collections.Generic;

// Token: 0x020000E0 RID: 224
public sealed class NoWallClingRegion : TrackTriggerObjects
{
	// Token: 0x17000087 RID: 135
	// (get) Token: 0x0600070A RID: 1802 RVA: 0x0002322F File Offset: 0x0002142F
	public static bool IsWallClingBlocked
	{
		get
		{
			return NoWallClingRegion.InsideRegions.Count > 0;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x0600070B RID: 1803 RVA: 0x0002323E File Offset: 0x0002143E
	protected override bool RequireEnabled
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00023241 File Offset: 0x00021441
	protected override void OnDisable()
	{
		base.OnDisable();
		NoWallClingRegion.InsideRegions.Remove(this);
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00023255 File Offset: 0x00021455
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside)
		{
			NoWallClingRegion.InsideRegions.Add(this);
			return;
		}
		NoWallClingRegion.InsideRegions.Remove(this);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00023274 File Offset: 0x00021474
	public static void RefreshInside()
	{
		NoWallClingRegion.RefreshRegions.AddRange(NoWallClingRegion.InsideRegions);
		foreach (NoWallClingRegion noWallClingRegion in NoWallClingRegion.RefreshRegions)
		{
			noWallClingRegion.Refresh();
		}
		NoWallClingRegion.RefreshRegions.Clear();
	}

	// Token: 0x040006E0 RID: 1760
	protected static HashSet<NoWallClingRegion> InsideRegions = new HashSet<NoWallClingRegion>();

	// Token: 0x040006E1 RID: 1761
	protected static List<NoWallClingRegion> RefreshRegions = new List<NoWallClingRegion>();
}
