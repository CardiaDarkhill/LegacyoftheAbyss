using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SecPlayerPrefs
{
	// Token: 0x02000833 RID: 2099
	public class SecureDataManager<T> where T : new()
	{
		// Token: 0x06004AB1 RID: 19121 RVA: 0x001624C4 File Offset: 0x001606C4
		public SecureDataManager(string filename)
		{
			this.key = filename;
			this.stats = this.Load();
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x001624DF File Offset: 0x001606DF
		public T Get()
		{
			return this.stats;
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x001624E8 File Offset: 0x001606E8
		private T Load()
		{
			if (!SecurePlayerPrefs.HasKey(this.key))
			{
				return Activator.CreateInstance<T>();
			}
			string @string = SecurePlayerPrefs.GetString(this.key);
			return this.DeserializeObject(@string);
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x0016251C File Offset: 0x0016071C
		public void Save(T stats)
		{
			string value = this.SerializeObject(stats);
			SecurePlayerPrefs.SetString(this.key, value);
			SecurePlayerPrefs.Save();
		}

		// Token: 0x06004AB5 RID: 19125 RVA: 0x00162544 File Offset: 0x00160744
		private string SerializeObject(T pObject)
		{
			Stream w = new MemoryStream();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			XmlTextWriter xmlTextWriter = new XmlTextWriter(w, Encoding.UTF8);
			xmlSerializer.Serialize(xmlTextWriter, pObject);
			return SecureDataManager<T>.UTF8ByteArrayToString(((MemoryStream)xmlTextWriter.BaseStream).ToArray());
		}

		// Token: 0x06004AB6 RID: 19126 RVA: 0x00162594 File Offset: 0x00160794
		private T DeserializeObject(string pXmlizedString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream(SecureDataManager<T>.StringToUTF8ByteArray(pXmlizedString));
			new XmlTextWriter(memoryStream, Encoding.UTF8);
			return (T)((object)xmlSerializer.Deserialize(memoryStream));
		}

		// Token: 0x06004AB7 RID: 19127 RVA: 0x001625D3 File Offset: 0x001607D3
		private static string UTF8ByteArrayToString(byte[] characters)
		{
			return new UTF8Encoding().GetString(characters);
		}

		// Token: 0x06004AB8 RID: 19128 RVA: 0x001625E0 File Offset: 0x001607E0
		private static byte[] StringToUTF8ByteArray(string pXmlString)
		{
			return new UTF8Encoding().GetBytes(pXmlString);
		}

		// Token: 0x04004A8E RID: 19086
		private T stats;

		// Token: 0x04004A8F RID: 19087
		private string key;
	}
}
