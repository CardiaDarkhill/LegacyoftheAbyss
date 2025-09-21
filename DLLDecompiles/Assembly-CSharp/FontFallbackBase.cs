using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200033A RID: 826
[Serializable]
public abstract class FontFallbackBase : ScriptableObject
{
	// Token: 0x06001CDB RID: 7387
	public abstract void OnChangedLanguage(LanguageCode newLanguage);
}
