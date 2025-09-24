using System;

// Token: 0x02000466 RID: 1126
[Serializable]
public sealed class RestorePointFileWrapper
{
	// Token: 0x06002862 RID: 10338 RVA: 0x000B2528 File Offset: 0x000B0728
	public RestorePointFileWrapper(byte[] data)
	{
		this.data = data;
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x000B2537 File Offset: 0x000B0737
	public RestorePointFileWrapper(byte[] data, int number)
	{
		this.data = data;
		this.number = number;
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000B254D File Offset: 0x000B074D
	public RestorePointFileWrapper(byte[] data, int number, string identifier)
	{
		this.data = data;
		this.number = number;
		this.identifier = identifier;
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000B256A File Offset: 0x000B076A
	public RestorePointFileWrapper()
	{
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000B2572 File Offset: 0x000B0772
	public void SetDateString()
	{
		this.date = RestorePointFileWrapper.GetDateString();
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000B257F File Offset: 0x000B077F
	public void SetVersion()
	{
		this.version = "1.0.28324";
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000B258C File Offset: 0x000B078C
	private static string GetDateString()
	{
		return DateTime.Now.ToString("yyyy/MM/dd");
	}

	// Token: 0x0400246A RID: 9322
	public byte[] data;

	// Token: 0x0400246B RID: 9323
	public string date;

	// Token: 0x0400246C RID: 9324
	public string version;

	// Token: 0x0400246D RID: 9325
	public int number;

	// Token: 0x0400246E RID: 9326
	public string identifier;
}
