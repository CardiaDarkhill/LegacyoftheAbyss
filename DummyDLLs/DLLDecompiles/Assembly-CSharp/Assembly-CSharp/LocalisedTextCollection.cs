using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001DD RID: 477
[CreateAssetMenu(menuName = "Hornet/Localised Text Collection")]
public class LocalisedTextCollection : ScriptableObject, ILocalisedTextCollection
{
	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06001293 RID: 4755 RVA: 0x00056585 File Offset: 0x00054785
	public bool IsActive
	{
		get
		{
			return this.data.IsActive;
		}
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x00056592 File Offset: 0x00054792
	public LocalisedString GetRandom(LocalisedString skipString)
	{
		return this.data.GetRandom(skipString);
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x000565A0 File Offset: 0x000547A0
	public NeedolinTextConfig GetConfig()
	{
		return this.data.GetConfig();
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x000565AD File Offset: 0x000547AD
	public LocalisedTextCollectionData ResolveAlternatives()
	{
		return this.data.ResolveAlternatives();
	}

	// Token: 0x04001160 RID: 4448
	[SerializeField]
	private LocalisedTextCollectionData data;
}
