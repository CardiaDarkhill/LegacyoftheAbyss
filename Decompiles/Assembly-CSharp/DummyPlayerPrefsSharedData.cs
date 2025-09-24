using System;

// Token: 0x0200045D RID: 1117
public class DummyPlayerPrefsSharedData : Platform.ISharedData
{
	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x06002817 RID: 10263 RVA: 0x000B1D50 File Offset: 0x000AFF50
	// (set) Token: 0x06002818 RID: 10264 RVA: 0x000B1D58 File Offset: 0x000AFF58
	public bool IsEncrypted { get; private set; }

	// Token: 0x06002819 RID: 10265 RVA: 0x000B1D61 File Offset: 0x000AFF61
	public DummyPlayerPrefsSharedData(bool isEncrypted)
	{
		this.IsEncrypted = isEncrypted;
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x000B1D70 File Offset: 0x000AFF70
	private string ReadEncrypted(string key)
	{
		return null;
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000B1D73 File Offset: 0x000AFF73
	private void WriteEncrypted(string key, string val)
	{
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x000B1D75 File Offset: 0x000AFF75
	public bool HasKey(string key)
	{
		return false;
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000B1D78 File Offset: 0x000AFF78
	public void DeleteKey(string key)
	{
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000B1D7A File Offset: 0x000AFF7A
	public void DeleteAll()
	{
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x000B1D7C File Offset: 0x000AFF7C
	public void ImportData(Platform.ISharedData otherData)
	{
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x000B1D7E File Offset: 0x000AFF7E
	public bool GetBool(string key, bool def)
	{
		return false;
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000B1D81 File Offset: 0x000AFF81
	public void SetBool(string key, bool val)
	{
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000B1D83 File Offset: 0x000AFF83
	public int GetInt(string key, int def)
	{
		return 0;
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000B1D86 File Offset: 0x000AFF86
	public void SetInt(string key, int val)
	{
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000B1D88 File Offset: 0x000AFF88
	public float GetFloat(string key, float def)
	{
		return 0f;
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000B1D8F File Offset: 0x000AFF8F
	public void SetFloat(string key, float val)
	{
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000B1D91 File Offset: 0x000AFF91
	public string GetString(string key, string def)
	{
		return string.Empty;
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x000B1D98 File Offset: 0x000AFF98
	public void SetString(string key, string val)
	{
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000B1D9A File Offset: 0x000AFF9A
	public void Save()
	{
	}
}
