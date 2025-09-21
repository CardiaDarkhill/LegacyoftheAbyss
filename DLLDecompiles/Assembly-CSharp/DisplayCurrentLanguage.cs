using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200040B RID: 1035
public class DisplayCurrentLanguage : MonoBehaviour
{
	// Token: 0x06002329 RID: 9001 RVA: 0x000A0DBB File Offset: 0x0009EFBB
	private void Awake()
	{
		if (!this.textObject)
		{
			this.textObject = base.GetComponent<Text>();
		}
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000A0DD8 File Offset: 0x0009EFD8
	private void OnEnable()
	{
		if (this.textObject)
		{
			string str = Language.CurrentLanguage().ToString();
			string arg = Language.Get("LANG_" + str, "MainMenu");
			this.textObject.text = string.Format(this.replaceText, arg);
		}
	}

	// Token: 0x040021D4 RID: 8660
	public Text textObject;

	// Token: 0x040021D5 RID: 8661
	public string replaceText = "({0})";
}
