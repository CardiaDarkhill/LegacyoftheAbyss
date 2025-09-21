using System;

// Token: 0x020000FA RID: 250
public class AtmosSnapshotMarker : SnapshotMarker
{
	// Token: 0x060007D8 RID: 2008 RVA: 0x0002589F File Offset: 0x00023A9F
	protected override void AddMarker()
	{
		AudioManager.AddAtmosMarker(this);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x000258A7 File Offset: 0x00023AA7
	protected override void RemoveMarker()
	{
		AudioManager.RemoveAtmosMarker(this);
	}
}
