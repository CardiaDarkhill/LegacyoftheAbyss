using System;
using System.IO;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public static class DemoHelper
{
	// Token: 0x170003AC RID: 940
	// (get) Token: 0x0600238D RID: 9101 RVA: 0x000A2B23 File Offset: 0x000A0D23
	public static bool IsDemoMode
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x0600238E RID: 9102 RVA: 0x000A2B26 File Offset: 0x000A0D26
	public static bool IsExhibitionMode
	{
		get
		{
			if (!DemoHelper.IsDemoMode)
			{
				return false;
			}
			if (DemoHelper._checkedExhibitionMode)
			{
				return DemoHelper._isExhibitionMode;
			}
			DemoHelper._checkedExhibitionMode = true;
			DemoHelper._isExhibitionMode = File.Exists(Path.Combine(Application.dataPath, "IsExhibitionMode"));
			return DemoHelper._isExhibitionMode;
		}
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000A2B64 File Offset: 0x000A0D64
	public static bool TryGetSaveData(int index, out string jsonData)
	{
		TextAsset saveFile = Demo.GetSaveFileOverride(index).SaveFile;
		if (saveFile != null)
		{
			jsonData = saveFile.text;
			return true;
		}
		jsonData = null;
		return false;
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000A2B94 File Offset: 0x000A0D94
	public static bool HasSaveFile(int saveSlot)
	{
		if (saveSlot == 0)
		{
			return true;
		}
		string text = null;
		return DemoHelper.TryGetSaveData(saveSlot - 1, out text);
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000A2BB2 File Offset: 0x000A0DB2
	public static bool IsDummySaveFile(int saveSlot)
	{
		return Demo.GetSaveFileOverride(saveSlot - 1).IsDummySave;
	}

	// Token: 0x04002246 RID: 8774
	private static bool _checkedExhibitionMode;

	// Token: 0x04002247 RID: 8775
	private static bool _isExhibitionMode;

	// Token: 0x04002248 RID: 8776
	private const string EXHIBITION_MODE_MARKER = "IsExhibitionMode";
}
