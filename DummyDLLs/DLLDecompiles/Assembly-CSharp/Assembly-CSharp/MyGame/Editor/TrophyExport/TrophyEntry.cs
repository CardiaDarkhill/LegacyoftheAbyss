using System;
using System.Xml.Serialization;

namespace MyGame.Editor.TrophyExport
{
	// Token: 0x0200089B RID: 2203
	[Serializable]
	public class TrophyEntry
	{
		// Token: 0x06004C47 RID: 19527 RVA: 0x00167719 File Offset: 0x00165919
		public TrophyEntry()
		{
		}

		// Token: 0x06004C48 RID: 19528 RVA: 0x00167721 File Offset: 0x00165921
		public TrophyEntry(int id, string name, string detail)
		{
			this.Id = id.ToString("D3");
			this.Name = name;
			this.Detail = detail;
		}

		// Token: 0x04004D9F RID: 19871
		[XmlAttribute("id")]
		public string Id;

		// Token: 0x04004DA0 RID: 19872
		[XmlElement("name")]
		public string Name;

		// Token: 0x04004DA1 RID: 19873
		[XmlElement("detail")]
		public string Detail;
	}
}
