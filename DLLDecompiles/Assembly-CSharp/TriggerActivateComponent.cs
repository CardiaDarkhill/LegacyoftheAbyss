using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200036F RID: 879
public class TriggerActivateComponent : MonoBehaviour
{
	// Token: 0x06001E27 RID: 7719 RVA: 0x0008B40F File Offset: 0x0008960F
	private void Start()
	{
		this.disableTimer = base.StartCoroutine(this.DisableTimer());
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x0008B423 File Offset: 0x00089623
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.component)
		{
			this.component.enabled = true;
		}
		if (this.disableTimer != null)
		{
			base.StopCoroutine(this.disableTimer);
			this.disableTimer = null;
		}
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x0008B459 File Offset: 0x00089659
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.disableTimer != null)
		{
			base.StopCoroutine(this.disableTimer);
		}
		this.disableTimer = base.StartCoroutine(this.DisableTimer());
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x0008B481 File Offset: 0x00089681
	private IEnumerator DisableTimer()
	{
		while (HeroController.instance && !HeroController.instance.isHeroInPosition)
		{
			yield return null;
		}
		yield return new WaitForSeconds(this.disableTime);
		if (this.component)
		{
			this.component.enabled = false;
		}
		yield break;
	}

	// Token: 0x04001D41 RID: 7489
	public MonoBehaviour component;

	// Token: 0x04001D42 RID: 7490
	public float disableTime = 1f;

	// Token: 0x04001D43 RID: 7491
	private Coroutine disableTimer;
}
