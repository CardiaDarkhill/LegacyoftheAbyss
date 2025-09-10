using System;

// Token: 0x02000181 RID: 385
public class FixedUpdateCache
{
	// Token: 0x06000C9D RID: 3229 RVA: 0x00038300 File Offset: 0x00036500
	public bool ShouldUpdate()
	{
		int fixedUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
		bool result = this.lastUpdate != fixedUpdateCycle;
		this.lastUpdate = fixedUpdateCycle;
		return result;
	}

	// Token: 0x04000C26 RID: 3110
	private int lastUpdate = -1;
}
