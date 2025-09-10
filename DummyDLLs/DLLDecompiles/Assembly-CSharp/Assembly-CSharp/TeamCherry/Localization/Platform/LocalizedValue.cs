using System;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	// Token: 0x020008B2 RID: 2226
	[Serializable]
	public class LocalizedValue
	{
		// Token: 0x04004DFC RID: 19964
		public string locale;

		// Token: 0x04004DFD RID: 19965
		[TextArea(1, 3)]
		public string text;
	}
}
