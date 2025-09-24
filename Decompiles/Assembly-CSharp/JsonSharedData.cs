using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000459 RID: 1113
public class JsonSharedData : CustomSharedData
{
	// Token: 0x0600276D RID: 10093 RVA: 0x000B0DF7 File Offset: 0x000AEFF7
	public JsonSharedData(string saveDir, string fileName, bool useEncryption)
	{
		this.saveDir = saveDir;
		this.dataPath = Path.Combine(saveDir, fileName);
		this.useEncryption = useEncryption;
		this.Load();
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000B0E20 File Offset: 0x000AF020
	public override void Save()
	{
		SaveDataUtility.AddTaskToAsyncQueue(delegate()
		{
			try
			{
				string text = base.SaveToJSON();
				byte[] bytes;
				if (this.useEncryption)
				{
					string graph = Encryption.Encrypt(text);
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream memoryStream = new MemoryStream();
					binaryFormatter.Serialize(memoryStream, graph);
					bytes = memoryStream.ToArray();
					memoryStream.Close();
				}
				else
				{
					bytes = Encoding.UTF8.GetBytes(text);
				}
				if (!Directory.Exists(this.saveDir))
				{
					Directory.CreateDirectory(this.saveDir);
				}
				File.WriteAllBytes(this.dataPath, bytes);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		});
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000B0E34 File Offset: 0x000AF034
	public void Load()
	{
		string path = this.dataPath;
		if (File.Exists(path))
		{
			try
			{
				byte[] array = File.ReadAllBytes(path);
				string str;
				if (this.useEncryption)
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream serializationStream = new MemoryStream(array);
					str = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
				}
				else
				{
					str = Encoding.UTF8.GetString(array);
				}
				base.LoadFromJSON(str);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	// Token: 0x04002432 RID: 9266
	private readonly bool useEncryption;

	// Token: 0x04002433 RID: 9267
	private readonly string saveDir;

	// Token: 0x04002434 RID: 9268
	private readonly string dataPath;
}
