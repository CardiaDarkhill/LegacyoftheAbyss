using System;
using System.Collections.Generic;

// Token: 0x020000DD RID: 221
public class NoClamberRegion : TrackTriggerObjects
{
	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060006F5 RID: 1781 RVA: 0x00022EF9 File Offset: 0x000210F9
	public static bool IsClamberBlocked
	{
		get
		{
			return NoClamberRegion.InsideRegions.Count > 0;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x00022F08 File Offset: 0x00021108
	protected override bool RequireEnabled
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00022F0B File Offset: 0x0002110B
	protected override void OnDisable()
	{
		base.OnDisable();
		NoClamberRegion.InsideRegions.Remove(this);
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00022F1F File Offset: 0x0002111F
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside)
		{
			NoClamberRegion.InsideRegions.Add(this);
			return;
		}
		NoClamberRegion.InsideRegions.Remove(this);
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00022F40 File Offset: 0x00021140
	public static void RefreshInside()
	{
		NoClamberRegion.RefreshRegions.AddRange(NoClamberRegion.InsideRegions);
		foreach (NoClamberRegion noClamberRegion in NoClamberRegion.RefreshRegions)
		{
			noClamberRegion.Refresh();
		}
		NoClamberRegion.RefreshRegions.Clear();
	}

	// Token: 0x040006D9 RID: 1753
	protected static HashSet<NoClamberRegion> InsideRegions = new HashSet<NoClamberRegion>();

	// Token: 0x040006DA RID: 1754
	protected static List<NoClamberRegion> RefreshRegions = new List<NoClamberRegion>();
}
