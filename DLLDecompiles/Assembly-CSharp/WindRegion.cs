using System;

// Token: 0x020000F4 RID: 244
public class WindRegion : TrackTriggerObjects
{
	// Token: 0x060007BA RID: 1978 RVA: 0x00025391 File Offset: 0x00023591
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (this.wasInside == isInside)
		{
			return;
		}
		this.wasInside = isInside;
		if (isInside)
		{
			WindRegion._insideRegions++;
		}
		else
		{
			WindRegion._insideRegions--;
		}
		WindRegion.UpdateWindy();
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x000253C6 File Offset: 0x000235C6
	public static void AddWind()
	{
		WindRegion._insideRegions++;
		WindRegion.UpdateWindy();
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x000253D9 File Offset: 0x000235D9
	public static void RemoveWind()
	{
		WindRegion._insideRegions--;
		WindRegion.UpdateWindy();
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x000253EC File Offset: 0x000235EC
	private static void UpdateWindy()
	{
		if (WindRegion._insideRegions == 1)
		{
			WindRegion.SetWindy(true);
			return;
		}
		if (WindRegion._insideRegions == 0)
		{
			WindRegion.SetWindy(false);
		}
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0002540C File Offset: 0x0002360C
	private static void SetWindy(bool value)
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		silentInstance.cState.inWindRegion = value;
	}

	// Token: 0x04000786 RID: 1926
	private static int _insideRegions;

	// Token: 0x04000787 RID: 1927
	private bool wasInside;
}
