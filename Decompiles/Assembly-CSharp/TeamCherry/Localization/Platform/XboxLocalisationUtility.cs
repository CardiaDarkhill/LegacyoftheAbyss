using System;
using System.IO;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	// Token: 0x020008AF RID: 2223
	public static class XboxLocalisationUtility
	{
		// Token: 0x04004DEE RID: 19950
		public static readonly string MAIN_DIRECTORY = Path.Combine(Application.dataPath, "Data Assets/Localisation");

		// Token: 0x04004DEF RID: 19951
		public static readonly string XML_DIRECTORY = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Xbox", "Online Config Backup");

		// Token: 0x04004DF0 RID: 19952
		public static readonly string ACHIEVEMENT_FILE_NAME = "achievements2017";

		// Token: 0x04004DF1 RID: 19953
		public static readonly string LOCALISATION_FILE_NAME = "localization";

		// Token: 0x04004DF2 RID: 19954
		public static readonly string ACHIEVEMENT_ASSET_PATH = "Assets/Data Assets/Localisation/Xbox Achievement Database.asset";

		// Token: 0x04004DF3 RID: 19955
		public static readonly string LOCALISATION_ASSET_PATH = "Assets/Data Assets/Localisation/Xbox Localisation Database.asset";
	}
}
