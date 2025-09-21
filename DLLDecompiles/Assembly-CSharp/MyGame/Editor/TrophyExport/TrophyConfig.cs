using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MyGame.Editor.TrophyExport
{
	// Token: 0x0200089A RID: 2202
	[XmlRoot("trophyconf")]
	[Serializable]
	public class TrophyConfig
	{
		// Token: 0x04004D99 RID: 19865
		[XmlAttribute("version")]
		public string Version = "1.1";

		// Token: 0x04004D9A RID: 19866
		[XmlAttribute("platform")]
		public string Platform = "ps4";

		// Token: 0x04004D9B RID: 19867
		[XmlAttribute("policy")]
		public string Policy = "large";

		// Token: 0x04004D9C RID: 19868
		[XmlElement("title-name")]
		public string TitleName = "";

		// Token: 0x04004D9D RID: 19869
		[XmlElement("title-detail")]
		public string TitleDetail = "";

		// Token: 0x04004D9E RID: 19870
		[XmlElement("trophy")]
		public List<TrophyEntry> Trophies = new List<TrophyEntry>();
	}
}
