using System;
using UnityEngine;

// Token: 0x020001DE RID: 478
[Serializable]
public class LocalisedTextCollectionField
{
	// Token: 0x06001298 RID: 4760 RVA: 0x000565C4 File Offset: 0x000547C4
	public ILocalisedTextCollection GetCollection()
	{
		if (!this.collection)
		{
			return this.customData;
		}
		return this.collection;
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x000565EF File Offset: 0x000547EF
	public void SetCollection(LocalisedTextCollection newCollection)
	{
		this.collection = newCollection;
	}

	// Token: 0x04001161 RID: 4449
	[SerializeField]
	private LocalisedTextCollection collection;

	// Token: 0x04001162 RID: 4450
	[SerializeField]
	[ModifiableProperty]
	[Conditional("collection", false, false, false)]
	private LocalisedTextCollectionData customData;
}
