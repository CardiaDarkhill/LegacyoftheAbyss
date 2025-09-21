using System;

// Token: 0x020000C3 RID: 195
public interface ITinkResponder
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600062E RID: 1582
	ITinkResponder.TinkFlags ResponderType { get; }

	// Token: 0x0600062F RID: 1583
	void Tinked();

	// Token: 0x02001432 RID: 5170
	[Flags]
	[Serializable]
	public enum TinkFlags
	{
		// Token: 0x0400825B RID: 33371
		None = 0,
		// Token: 0x0400825C RID: 33372
		Normal = 1,
		// Token: 0x0400825D RID: 33373
		Projectile = 2,
		// Token: 0x0400825E RID: 33374
		Unused = -2147483648,
		// Token: 0x0400825F RID: 33375
		All = 3
	}
}
