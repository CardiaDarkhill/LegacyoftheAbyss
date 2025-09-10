using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200044F RID: 1103
public abstract class CustomSharedData : Platform.ISharedData
{
	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060026B8 RID: 9912 RVA: 0x000AF22A File Offset: 0x000AD42A
	public Dictionary<string, string> SharedData
	{
		get
		{
			return this.sharedData;
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x060026B9 RID: 9913 RVA: 0x000AF232 File Offset: 0x000AD432
	// (set) Token: 0x060026BA RID: 9914 RVA: 0x000AF23A File Offset: 0x000AD43A
	public CustomSharedData.IResponder Responder
	{
		get
		{
			return this.responder;
		}
		set
		{
			this.responder = value;
		}
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x000AF243 File Offset: 0x000AD443
	protected CustomSharedData()
	{
		this.sharedData = new Dictionary<string, string>();
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x000AF256 File Offset: 0x000AD456
	public void LoadFromJSON(string str)
	{
		JsonUtility.FromJson<CustomSharedData.SharedDataSerializableBlob>(str).ToSharedData(this.sharedData);
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000AF269 File Offset: 0x000AD469
	public string SaveToJSON()
	{
		return JsonUtility.ToJson(CustomSharedData.SharedDataSerializableBlob.FromSharedData(this.sharedData));
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000AF27C File Offset: 0x000AD47C
	protected static byte[] JsonToBytes(string jsonData, bool useEncryption)
	{
		byte[] result;
		if (useEncryption)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000AF2C4 File Offset: 0x000AD4C4
	protected static string BytesToJson(byte[] byteData, bool useEncryption)
	{
		string result;
		if (useEncryption)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(byteData);
			result = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		else
		{
			result = Encoding.UTF8.GetString(byteData);
		}
		return result;
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000AF300 File Offset: 0x000AD500
	public bool HasKey(string key)
	{
		return this.sharedData.ContainsKey(key);
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000AF30E File Offset: 0x000AD50E
	public void DeleteKey(string key)
	{
		if (this.sharedData.Remove(key))
		{
			this.OnModified();
		}
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000AF324 File Offset: 0x000AD524
	public void DeleteAll()
	{
		if (this.sharedData.Count > 0)
		{
			this.sharedData.Clear();
			this.OnModified();
		}
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000AF348 File Offset: 0x000AD548
	public virtual void ImportData(Platform.ISharedData otherData)
	{
		if (otherData == null)
		{
			return;
		}
		CustomSharedData customSharedData = otherData as CustomSharedData;
		if (customSharedData != null)
		{
			foreach (KeyValuePair<string, string> keyValuePair in customSharedData.sharedData)
			{
				if (!this.sharedData.ContainsKey(keyValuePair.Key))
				{
					this.sharedData.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000AF3D0 File Offset: 0x000AD5D0
	public bool GetBool(string key, bool def)
	{
		return this.GetInt(key, def ? 1 : 0) > 0;
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x000AF3E3 File Offset: 0x000AD5E3
	public void SetBool(string key, bool val)
	{
		this.SetInt(key, val ? 1 : 0);
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000AF3F4 File Offset: 0x000AD5F4
	public int GetInt(string key, int def)
	{
		string s;
		if (!this.sharedData.TryGetValue(key, out s))
		{
			return def;
		}
		int result;
		if (!int.TryParse(s, out result))
		{
			return def;
		}
		return result;
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000AF420 File Offset: 0x000AD620
	public void SetInt(string key, int val)
	{
		string text = val.ToString();
		if (!this.sharedData.ContainsKey(key) || this.sharedData[key] != text)
		{
			this.sharedData[key] = text;
			this.OnModified();
		}
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000AF46C File Offset: 0x000AD66C
	public float GetFloat(string key, float def)
	{
		string s;
		if (!this.sharedData.TryGetValue(key, out s))
		{
			return def;
		}
		float result;
		if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
		{
			return def;
		}
		return result;
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000AF4A4 File Offset: 0x000AD6A4
	public void SetFloat(string key, float val)
	{
		string text = val.ToString(CultureInfo.InvariantCulture);
		if (!this.sharedData.ContainsKey(key) || this.sharedData[key] != text)
		{
			this.sharedData[key] = text;
			this.OnModified();
		}
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000AF4F4 File Offset: 0x000AD6F4
	public string GetString(string key, string def)
	{
		string result;
		if (!this.sharedData.TryGetValue(key, out result))
		{
			return def;
		}
		return result;
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000AF514 File Offset: 0x000AD714
	public void SetString(string key, string val)
	{
		if (!this.sharedData.ContainsKey(key) || this.sharedData[key] != val)
		{
			this.sharedData[key] = val;
			this.OnModified();
		}
	}

	// Token: 0x060026CC RID: 9932
	public abstract void Save();

	// Token: 0x060026CD RID: 9933 RVA: 0x000AF54B File Offset: 0x000AD74B
	protected virtual void OnModified()
	{
		if (this.responder != null)
		{
			this.responder.OnModified(this);
		}
	}

	// Token: 0x04002411 RID: 9233
	private Dictionary<string, string> sharedData;

	// Token: 0x04002412 RID: 9234
	private CustomSharedData.IResponder responder;

	// Token: 0x02001737 RID: 5943
	[Serializable]
	private class SharedDataSerializableBlob
	{
		// Token: 0x06008D1A RID: 36122 RVA: 0x00288FB8 File Offset: 0x002871B8
		public static CustomSharedData.SharedDataSerializableBlob FromSharedData(Dictionary<string, string> sharedData)
		{
			List<CustomSharedData.SharedDataSerializablePair> list = new List<CustomSharedData.SharedDataSerializablePair>();
			foreach (KeyValuePair<string, string> keyValuePair in sharedData)
			{
				if (keyValuePair.Key == null)
				{
					Debug.LogErrorFormat("Null key found in shared data", Array.Empty<object>());
				}
				else if (keyValuePair.Value == null)
				{
					Debug.LogErrorFormat("Null value for key '{0}' found in shared data", Array.Empty<object>());
				}
				else
				{
					list.Add(new CustomSharedData.SharedDataSerializablePair
					{
						Key = keyValuePair.Key,
						Value = keyValuePair.Value
					});
				}
			}
			return new CustomSharedData.SharedDataSerializableBlob
			{
				pairs = list.ToArray()
			};
		}

		// Token: 0x06008D1B RID: 36123 RVA: 0x00289078 File Offset: 0x00287278
		public void ToSharedData(Dictionary<string, string> sharedData)
		{
			sharedData.Clear();
			int num = 0;
			while (this.pairs != null && num < this.pairs.Length)
			{
				CustomSharedData.SharedDataSerializablePair sharedDataSerializablePair = this.pairs[num];
				if (sharedDataSerializablePair.Key == null)
				{
					Debug.LogErrorFormat("Null key found in shared data", Array.Empty<object>());
				}
				else if (sharedDataSerializablePair.Value == null)
				{
					Debug.LogErrorFormat("Null value for key '{0}' found in shared data", new object[]
					{
						sharedDataSerializablePair.Key
					});
				}
				else if (sharedData.ContainsKey(sharedDataSerializablePair.Key))
				{
					Debug.LogErrorFormat("Duplicate key '{0}' found in shared data", new object[]
					{
						sharedDataSerializablePair.Key
					});
				}
				else
				{
					sharedData.Add(sharedDataSerializablePair.Key, sharedDataSerializablePair.Value);
				}
				num++;
			}
		}

		// Token: 0x04008DA7 RID: 36263
		[SerializeField]
		private CustomSharedData.SharedDataSerializablePair[] pairs;
	}

	// Token: 0x02001738 RID: 5944
	[Serializable]
	private struct SharedDataSerializablePair
	{
		// Token: 0x04008DA8 RID: 36264
		public string Key;

		// Token: 0x04008DA9 RID: 36265
		public string Value;
	}

	// Token: 0x02001739 RID: 5945
	public interface IResponder
	{
		// Token: 0x06008D1D RID: 36125
		void OnModified(CustomSharedData sharedData);
	}
}
