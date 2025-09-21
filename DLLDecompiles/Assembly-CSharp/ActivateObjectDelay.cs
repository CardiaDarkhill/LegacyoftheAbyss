using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class ActivateObjectDelay : MonoBehaviour
{
	// Token: 0x06000226 RID: 550 RVA: 0x0000D9BD File Offset: 0x0000BBBD
	private void OnEnable()
	{
		this.timer = this.time;
		this.didActivation = false;
		this.objectToActivate.SetActive(false);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000D9DE File Offset: 0x0000BBDE
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

	// Token: 0x06000228 RID: 552 RVA: 0x0000DA1B File Offset: 0x0000BC1B
	public void Cancel()
	{
		this.didActivation = true;
	}

	// Token: 0x040001D7 RID: 471
	[SerializeField]
	private GameObject objectToActivate;

	// Token: 0x040001D8 RID: 472
	[SerializeField]
	private float time;

	// Token: 0x040001D9 RID: 473
	private float timer;

	// Token: 0x040001DA RID: 474
	private bool didActivation;
}
