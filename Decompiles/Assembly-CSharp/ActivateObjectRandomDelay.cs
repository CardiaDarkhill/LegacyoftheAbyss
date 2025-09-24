using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class ActivateObjectRandomDelay : MonoBehaviour
{
	// Token: 0x0600022A RID: 554 RVA: 0x0000DA2C File Offset: 0x0000BC2C
	private void OnEnable()
	{
		this.time = Random.Range(this.timeMin, this.timeMax);
		this.timer = this.time;
		this.didActivation = false;
		this.objectToActivate.SetActive(false);
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000DA64 File Offset: 0x0000BC64
	private void Update()
	{
		if (!this.didActivation)
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.objectToActivate.SetActive(true);
			this.didActivation = true;
		}
	}

	// Token: 0x040001DB RID: 475
	public GameObject objectToActivate;

	// Token: 0x040001DC RID: 476
	public float timeMin;

	// Token: 0x040001DD RID: 477
	public float timeMax;

	// Token: 0x040001DE RID: 478
	private float time;

	// Token: 0x040001DF RID: 479
	private float timer;

	// Token: 0x040001E0 RID: 480
	private bool didActivation;
}
