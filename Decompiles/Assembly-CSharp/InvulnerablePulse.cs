using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class InvulnerablePulse : MonoBehaviour
{
	// Token: 0x06000499 RID: 1177 RVA: 0x00018BD0 File Offset: 0x00016DD0
	private void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00018BDE File Offset: 0x00016DDE
	[ContextMenu("Start Flash")]
	public void StartInvulnerablePulse()
	{
		this.StopInvulnerablePulse();
		this.flashRoutine = base.StartCoroutine(this.Flash());
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00018BF8 File Offset: 0x00016DF8
	[ContextMenu("Stop Flash")]
	public void StopInvulnerablePulse()
	{
		if (this.flashRoutine != null)
		{
			base.StopCoroutine(this.flashRoutine);
			this.flashRoutine = null;
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00018C15 File Offset: 0x00016E15
	private IEnumerator Flash()
	{
		WaitForSeconds wait = new WaitForSeconds(this.pulseDuration * 2f);
		for (;;)
		{
			this.spriteFlash.Flash(this.flashColour, this.flashAmount, this.pulseDuration, 0f, this.pulseDuration, 0f, false, 0, 0, false);
			yield return wait;
		}
		yield break;
	}

	// Token: 0x04000459 RID: 1113
	public Color flashColour = Color.white;

	// Token: 0x0400045A RID: 1114
	public float flashAmount = 0.85f;

	// Token: 0x0400045B RID: 1115
	public float pulseDuration;

	// Token: 0x0400045C RID: 1116
	private SpriteFlash spriteFlash;

	// Token: 0x0400045D RID: 1117
	private Coroutine flashRoutine;
}
