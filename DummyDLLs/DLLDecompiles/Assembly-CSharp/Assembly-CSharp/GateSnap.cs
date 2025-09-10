using System;
using UnityEngine;

// Token: 0x020005D7 RID: 1495
[ExecuteInEditMode]
public class GateSnap : MonoBehaviour
{
	// Token: 0x0600351B RID: 13595 RVA: 0x000EB960 File Offset: 0x000E9B60
	private void Update()
	{
		Vector2 vector = base.transform.position;
		vector.x = Mathf.Round(vector.x / this.snapX) * this.snapX;
		vector.y = Mathf.Round(vector.y / this.snapY) * this.snapY;
		base.transform.position = vector;
	}

	// Token: 0x0400387C RID: 14460
	private float snapX = 0.5f;

	// Token: 0x0400387D RID: 14461
	private float snapY = 0.5f;
}
