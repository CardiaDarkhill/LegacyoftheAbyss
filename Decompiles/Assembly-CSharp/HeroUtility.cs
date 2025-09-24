using System;

// Token: 0x02000194 RID: 404
public static class HeroUtility
{
	// Token: 0x06000F9B RID: 3995 RVA: 0x0004B4C1 File Offset: 0x000496C1
	public static void Reset()
	{
		HeroUtility.cancellables.FullClear();
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0004B4CD File Offset: 0x000496CD
	public static void AddCancellable(ICancellable cancellable)
	{
		HeroUtility.cancellables.Add(cancellable);
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0004B4DB File Offset: 0x000496DB
	public static void RemoveCancellable(ICancellable cancellable)
	{
		HeroUtility.cancellables.Remove(cancellable);
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0004B4EC File Offset: 0x000496EC
	public static void CancelCancellables()
	{
		HeroUtility.cancellables.ReserveListUsage();
		foreach (ICancellable cancellable in HeroUtility.cancellables.List)
		{
			cancellable.DoCancellation();
		}
		HeroUtility.cancellables.ReleaseListUsage();
	}

	// Token: 0x04000F3A RID: 3898
	private static UniqueList<ICancellable> cancellables = new UniqueList<ICancellable>();
}
