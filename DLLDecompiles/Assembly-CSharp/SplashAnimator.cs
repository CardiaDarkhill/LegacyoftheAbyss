using System;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class SplashAnimator : MonoBehaviour
{
	// Token: 0x06000547 RID: 1351 RVA: 0x0001AD24 File Offset: 0x00018F24
	private void OnEnable()
	{
		float num = Random.Range(this.scaleMin, this.scaleMax);
		base.transform.localScale = new Vector3(num, num, num);
		if ((float)Random.Range(0, 100) < 50f)
		{
			base.transform.localScale = new Vector3(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}

	// Token: 0x0400051D RID: 1309
	public float scaleMin;

	// Token: 0x0400051E RID: 1310
	public float scaleMax;
}
