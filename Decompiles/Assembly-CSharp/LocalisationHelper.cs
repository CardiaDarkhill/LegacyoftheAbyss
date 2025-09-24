using System;
using System.Collections.Generic;

// Token: 0x0200040C RID: 1036
public static class LocalisationHelper
{
	// Token: 0x0600232C RID: 9004 RVA: 0x000A0E48 File Offset: 0x0009F048
	public static string GetProcessed(this string text, LocalisationHelper.FontSource fontSource)
	{
		if (LocalisationHelper.substitutions.ContainsKey(fontSource))
		{
			string text2 = text;
			foreach (KeyValuePair<string, string> keyValuePair in LocalisationHelper.substitutions[fontSource])
			{
				text2 = text2.Replace(keyValuePair.Key, keyValuePair.Value);
			}
			if (text2 != text)
			{
				text = text2;
			}
		}
		return text;
	}

	// Token: 0x040021D6 RID: 8662
	private static Dictionary<LocalisationHelper.FontSource, Dictionary<string, string>> substitutions = new Dictionary<LocalisationHelper.FontSource, Dictionary<string, string>>
	{
		{
			LocalisationHelper.FontSource.Trajan,
			new Dictionary<string, string>
			{
				{
					"ß",
					"ss"
				}
			}
		}
	};

	// Token: 0x020016A2 RID: 5794
	public enum FontSource
	{
		// Token: 0x04008B94 RID: 35732
		Trajan,
		// Token: 0x04008B95 RID: 35733
		Perpetua
	}
}
