using System;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class TextMeshOrderingLayer : MonoBehaviour
{
	// Token: 0x060034E0 RID: 13536 RVA: 0x000EAAAC File Offset: 0x000E8CAC
	private void Start()
	{
		base.GetComponent<MeshRenderer>().sortingLayerID = base.transform.parent.GetComponent<SpriteRenderer>().sortingLayerID;
		base.GetComponent<MeshRenderer>().sortingOrder = base.transform.parent.GetComponent<SpriteRenderer>().sortingOrder;
	}
}
