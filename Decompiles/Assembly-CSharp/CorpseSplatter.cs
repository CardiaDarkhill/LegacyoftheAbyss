using System;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public class CorpseSplatter : MonoBehaviour
{
	// Token: 0x060018A7 RID: 6311 RVA: 0x0007108B File Offset: 0x0006F28B
	private void Awake()
	{
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.splatEffect, 2, true, false, false);
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x000710A4 File Offset: 0x0006F2A4
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer != 26)
		{
			return;
		}
		ActiveCorpse component = other.GetComponent<ActiveCorpse>();
		if (!component)
		{
			return;
		}
		component.gameObject.SetActive(false);
		this.splatEffect.Spawn(component.transform.position);
	}

	// Token: 0x040017A3 RID: 6051
	[SerializeField]
	private GameObject splatEffect;
}
