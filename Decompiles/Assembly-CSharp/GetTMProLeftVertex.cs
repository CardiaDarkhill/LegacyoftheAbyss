using System;
using TMProOld;
using UnityEngine;

// Token: 0x0200066F RID: 1647
public class GetTMProLeftVertex : MonoBehaviour
{
	// Token: 0x06003B20 RID: 15136 RVA: 0x001049DA File Offset: 0x00102BDA
	private void Start()
	{
		this.textMesh = base.GetComponent<TextMeshPro>();
	}

	// Token: 0x06003B21 RID: 15137 RVA: 0x001049E8 File Offset: 0x00102BE8
	public float GetLeftmostVertex()
	{
		return this.textMesh.mesh.bounds.extents.x;
	}

	// Token: 0x04003D71 RID: 15729
	private TextMeshPro textMesh;
}
