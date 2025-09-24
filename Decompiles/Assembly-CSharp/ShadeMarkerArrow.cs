using System;
using GlobalEnums;

// Token: 0x0200071D RID: 1821
public class ShadeMarkerArrow : MapMarkerArrow
{
	// Token: 0x060040BE RID: 16574 RVA: 0x0011C852 File Offset: 0x0011AA52
	protected override bool IsActive(bool isQuickMap, MapZone currentMapZone)
	{
		return this.isActive;
	}

	// Token: 0x060040BF RID: 16575 RVA: 0x0011C85A File Offset: 0x0011AA5A
	public void SetActive(bool value)
	{
		this.isActive = value;
	}

	// Token: 0x04004243 RID: 16963
	private bool isActive;
}
