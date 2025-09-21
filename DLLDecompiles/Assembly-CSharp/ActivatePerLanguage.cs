using System;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class ActivatePerLanguage : MonoBehaviour
{
	// Token: 0x060022CA RID: 8906 RVA: 0x0009FC69 File Offset: 0x0009DE69
	private void Start()
	{
		this.UpdateLanguage();
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x0009FC74 File Offset: 0x0009DE74
	public void UpdateLanguage()
	{
		SupportedLanguages supportedLanguages = (SupportedLanguages)Language.CurrentLanguage();
		bool activate = this.defaultActivation;
		foreach (ActivatePerLanguage.LangBoolPair langBoolPair in this.languages)
		{
			if (langBoolPair.language == supportedLanguages)
			{
				activate = langBoolPair.activate;
				break;
			}
		}
		if (this.target)
		{
			this.target.SetActive(activate);
		}
		if (this.alt)
		{
			this.alt.SetActive(!activate);
		}
	}

	// Token: 0x0400219A RID: 8602
	public GameObject target;

	// Token: 0x0400219B RID: 8603
	public GameObject alt;

	// Token: 0x0400219C RID: 8604
	[Space]
	public ActivatePerLanguage.LangBoolPair[] languages;

	// Token: 0x0400219D RID: 8605
	[Space]
	public bool defaultActivation = true;

	// Token: 0x02001695 RID: 5781
	[Serializable]
	public struct LangBoolPair
	{
		// Token: 0x04008B79 RID: 35705
		public SupportedLanguages language;

		// Token: 0x04008B7A RID: 35706
		public bool activate;
	}
}
