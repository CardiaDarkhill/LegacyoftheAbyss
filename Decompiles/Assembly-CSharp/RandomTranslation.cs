using System;
using UnityEngine;

// Token: 0x020005B2 RID: 1458
public class RandomTranslation : MonoBehaviour
{
	// Token: 0x06003465 RID: 13413 RVA: 0x000E8DB3 File Offset: 0x000E6FB3
	private void OnEnable()
	{
		this.shifted = false;
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x000E8DBC File Offset: 0x000E6FBC
	private void LateUpdate()
	{
		if (!this.shifted)
		{
			this.DoShift();
			this.shifted = true;
		}
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x000E8DD4 File Offset: 0x000E6FD4
	private void DoShift()
	{
		base.transform.position = new Vector3(base.transform.position.x + Random.Range(-this.xRange, this.xRange), base.transform.position.y + Random.Range(-this.yRange, this.yRange), base.transform.position.z + Random.Range(-this.zRange, this.zRange));
	}

	// Token: 0x040037E3 RID: 14307
	public float xRange;

	// Token: 0x040037E4 RID: 14308
	public float yRange;

	// Token: 0x040037E5 RID: 14309
	public float zRange;

	// Token: 0x040037E6 RID: 14310
	private bool shifted;
}
