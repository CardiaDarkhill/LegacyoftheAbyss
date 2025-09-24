using System;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class ChainLinkTerrainFixer : MonoBehaviour
{
	// Token: 0x06002B60 RID: 11104 RVA: 0x000BE5CD File Offset: 0x000BC7CD
	private void OnCollisionEnter2D()
	{
		if (this.chainLink)
		{
			this.chainLink.SetActive(false);
		}
	}

	// Token: 0x04002CB3 RID: 11443
	[SerializeField]
	private ChainLinkInteraction chainLink;
}
