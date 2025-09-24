using System;
using UnityEngine;

// Token: 0x020005D9 RID: 1497
public class ParallaxSorter : MonoBehaviour
{
	// Token: 0x0600352D RID: 13613 RVA: 0x000EBE00 File Offset: 0x000EA000
	private void Awake()
	{
		this.stripSortingLayers = true;
	}

	// Token: 0x04003891 RID: 14481
	public string[] sortingLayers;

	// Token: 0x04003892 RID: 14482
	public int[] sortingLayerIDs;

	// Token: 0x04003893 RID: 14483
	public float[] layerDepths;

	// Token: 0x04003894 RID: 14484
	public bool stripSortingLayers;
}
