using System;

// Token: 0x02000362 RID: 866
public interface IBreakableProjectile
{
	// Token: 0x06001DF1 RID: 7665
	void QueueBreak(IBreakableProjectile.HitInfo hitInfo);

	// Token: 0x02001617 RID: 5655
	public struct HitInfo
	{
		// Token: 0x040089AD RID: 35245
		public bool isWall;
	}
}
