using System;
using UnityEngine;

// Token: 0x0200076A RID: 1898
public class GuidReferenceHolder : MonoBehaviour
{
	// Token: 0x17000797 RID: 1943
	// (get) Token: 0x060043AC RID: 17324 RVA: 0x00129715 File Offset: 0x00127915
	public GameObject ReferencedGameObject
	{
		get
		{
			if (!this.localOverride)
			{
				return this.reference.gameObject;
			}
			return this.localOverride;
		}
	}

	// Token: 0x04004528 RID: 17704
	[SerializeField]
	private GameObject localOverride;

	// Token: 0x04004529 RID: 17705
	[SerializeField]
	private GuidReference reference;
}
