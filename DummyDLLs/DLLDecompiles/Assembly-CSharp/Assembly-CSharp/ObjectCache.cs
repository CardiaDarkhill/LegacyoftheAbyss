using System;

// Token: 0x020001CB RID: 459
public sealed class ObjectCache<T>
{
	// Token: 0x170001EB RID: 491
	// (get) Token: 0x060011F8 RID: 4600 RVA: 0x00053C92 File Offset: 0x00051E92
	public T Value
	{
		get
		{
			return this.cache;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x060011F9 RID: 4601 RVA: 0x00053C9A File Offset: 0x00051E9A
	// (set) Token: 0x060011FA RID: 4602 RVA: 0x00053CA2 File Offset: 0x00051EA2
	public int Version { get; private set; }

	// Token: 0x060011FB RID: 4603 RVA: 0x00053CAB File Offset: 0x00051EAB
	public void UpdateCache(T update, int version)
	{
		this.cache = update;
		this.Version = version;
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x00053CBB File Offset: 0x00051EBB
	public bool ShouldUpdate(int version)
	{
		return this.Version != version;
	}

	// Token: 0x040010CD RID: 4301
	private T cache;
}
