using System;
using System.Globalization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200045C RID: 1116
public class PlayerPrefsSharedData : Platform.ISharedData
{
	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x06002805 RID: 10245 RVA: 0x000B1B89 File Offset: 0x000AFD89
	// (set) Token: 0x06002806 RID: 10246 RVA: 0x000B1B91 File Offset: 0x000AFD91
	public bool IsEncrypted { get; private set; }

	// Token: 0x06002807 RID: 10247 RVA: 0x000B1B9A File Offset: 0x000AFD9A
	public PlayerPrefsSharedData(bool isEncrypted)
	{
		this.IsEncrypted = isEncrypted;
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000B1BAC File Offset: 0x000AFDAC
	private string ReadEncrypted(string key)
	{
		string @string = PlayerPrefs.GetString(Encryption.Encrypt(key), string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		return Encryption.Decrypt(@string);
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000B1BDC File Offset: 0x000AFDDC
	private void WriteEncrypted(string key, string val)
	{
		string key2 = Encryption.Encrypt(key);
		string value = Encryption.Encrypt(val);
		PlayerPrefs.SetString(key2, value);
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000B1BFC File Offset: 0x000AFDFC
	public bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000B1C04 File Offset: 0x000AFE04
	public void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000B1C0C File Offset: 0x000AFE0C
	public void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000B1C13 File Offset: 0x000AFE13
	public void ImportData(Platform.ISharedData otherData)
	{
		Debug.LogError("PlayerPrefsSharedData does not support ImportData");
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000B1C1F File Offset: 0x000AFE1F
	public bool GetBool(string key, bool def)
	{
		return this.GetInt(key, def ? 1 : 0) > 0;
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000B1C32 File Offset: 0x000AFE32
	public void SetBool(string key, bool val)
	{
		this.SetInt(key, val ? 1 : 0);
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000B1C44 File Offset: 0x000AFE44
	public int GetInt(string key, int def)
	{
		if (!this.IsEncrypted)
		{
			return PlayerPrefs.GetInt(key, def);
		}
		string text = this.ReadEncrypted(key);
		if (text == null)
		{
			return def;
		}
		int result;
		if (!int.TryParse(text, out result))
		{
			return def;
		}
		return result;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000B1C7B File Offset: 0x000AFE7B
	public void SetInt(string key, int val)
	{
		if (this.IsEncrypted)
		{
			this.WriteEncrypted(key, val.ToString());
			return;
		}
		PlayerPrefs.SetInt(key, val);
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000B1C9C File Offset: 0x000AFE9C
	public float GetFloat(string key, float def)
	{
		if (!this.IsEncrypted)
		{
			return PlayerPrefs.GetFloat(key, def);
		}
		string text = this.ReadEncrypted(key);
		if (text == null)
		{
			return def;
		}
		float result;
		if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
		{
			return def;
		}
		return result;
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000B1CDD File Offset: 0x000AFEDD
	public void SetFloat(string key, float val)
	{
		if (this.IsEncrypted)
		{
			this.WriteEncrypted(key, val.ToString(CultureInfo.InvariantCulture));
			return;
		}
		PlayerPrefs.SetFloat(key, val);
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000B1D04 File Offset: 0x000AFF04
	public string GetString(string key, string def)
	{
		if (!this.IsEncrypted)
		{
			return PlayerPrefs.GetString(key, def);
		}
		string text = this.ReadEncrypted(key);
		if (text == null)
		{
			return def;
		}
		return text;
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x000B1D2F File Offset: 0x000AFF2F
	public void SetString(string key, string val)
	{
		if (this.IsEncrypted)
		{
			this.WriteEncrypted(key, val);
			return;
		}
		PlayerPrefs.SetString(key, val);
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000B1D49 File Offset: 0x000AFF49
	public virtual void Save()
	{
		PlayerPrefs.Save();
	}
}
