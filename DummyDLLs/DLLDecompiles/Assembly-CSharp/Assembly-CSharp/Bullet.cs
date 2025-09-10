using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class Bullet : MonoBehaviour
{
	// Token: 0x060000AE RID: 174 RVA: 0x00005286 File Offset: 0x00003486
	private void OnEnable()
	{
		base.StartCoroutine(this.Shoot());
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005295 File Offset: 0x00003495
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x0000529D File Offset: 0x0000349D
	private IEnumerator Shoot()
	{
		float travelledDistance = 0f;
		while (travelledDistance < this.shootDistance)
		{
			travelledDistance += this.shootSpeed * Time.deltaTime;
			base.transform.position += base.transform.forward * (this.shootSpeed * Time.deltaTime);
			yield return 0;
		}
		this.explosionPrefab.Spawn(base.transform.position);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x0400009D RID: 157
	public Explosion explosionPrefab;

	// Token: 0x0400009E RID: 158
	public float shootDistance;

	// Token: 0x0400009F RID: 159
	public float shootSpeed;
}
