using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200040F RID: 1039
[RequireComponent(typeof(AutoLocalizeTextUI))]
public class PlatformSpecificLocalisation : MonoBehaviour
{
	// Token: 0x06002335 RID: 9013 RVA: 0x000A0FF8 File Offset: 0x0009F1F8
	private void OnValidate()
	{
		for (int i = 0; i < this.platformKeys.Length; i++)
		{
			PlatformSpecificLocalisation.PlatformKey platformKey = this.platformKeys[i];
			if (!string.IsNullOrEmpty(platformKey.sheetTitle) || !string.IsNullOrEmpty(platformKey.textKey))
			{
				platformKey.text = new LocalisedString(platformKey.sheetTitle, platformKey.textKey);
				platformKey.sheetTitle = string.Empty;
				platformKey.textKey = string.Empty;
				this.platformKeys[i] = platformKey;
			}
		}
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000A107C File Offset: 0x0009F27C
	private void Awake()
	{
		this.OnValidate();
		this.localisation = base.GetComponent<AutoLocalizeTextUI>();
		if (this.localisation)
		{
			foreach (PlatformSpecificLocalisation.PlatformKey platformKey in this.platformKeys)
			{
				if (platformKey.platform == Application.platform)
				{
					this.localisation.Text = platformKey.text;
					return;
				}
			}
		}
	}

	// Token: 0x040021DD RID: 8669
	public PlatformSpecificLocalisation.PlatformKey[] platformKeys;

	// Token: 0x040021DE RID: 8670
	private AutoLocalizeTextUI localisation;

	// Token: 0x020016A4 RID: 5796
	[Serializable]
	public struct PlatformKey
	{
		// Token: 0x04008B98 RID: 35736
		public RuntimePlatform platform;

		// Token: 0x04008B99 RID: 35737
		public LocalisedString text;

		// Token: 0x04008B9A RID: 35738
		[HideInInspector]
		public string sheetTitle;

		// Token: 0x04008B9B RID: 35739
		[HideInInspector]
		public string textKey;
	}
}
