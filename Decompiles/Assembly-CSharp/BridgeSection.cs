using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class BridgeSection : MonoBehaviour
{
	// Token: 0x06000A1A RID: 2586 RVA: 0x0002DC76 File Offset: 0x0002BE76
	private void Awake()
	{
		base.transform.SetPositionZ(0.036f);
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0002DC88 File Offset: 0x0002BE88
	public void Open(BridgeLever lever, bool playAnim = true)
	{
		if (playAnim)
		{
			float num = Vector2.Distance(base.transform.position, lever.transform.position);
			base.StartCoroutine(this.Open(num * 0.1f + 0.25f));
			return;
		}
		this.sectionAnim.Play("Bridge Activated");
		this.fenceAnim.Play("Fence Activated");
		this.fenceRenderer.enabled = true;
		base.transform.SetPositionZ(0.001f);
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0002DD15 File Offset: 0x0002BF15
	private IEnumerator Open(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		base.StartCoroutine(this.OpenFence());
		this.sectionAnim.Play("Bridge Rise");
		this.source.Play();
		base.transform.SetPositionZ(0.001f);
		yield break;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0002DD2B File Offset: 0x0002BF2B
	private IEnumerator OpenFence()
	{
		yield return new WaitForSeconds(2.5f);
		this.fenceRenderer.enabled = true;
		this.fenceAnim.Play("Fence Rise");
		this.fenceSource.Play();
		yield break;
	}

	// Token: 0x040009AE RID: 2478
	public tk2dSpriteAnimator sectionAnim;

	// Token: 0x040009AF RID: 2479
	public tk2dSpriteAnimator fenceAnim;

	// Token: 0x040009B0 RID: 2480
	public MeshRenderer fenceRenderer;

	// Token: 0x040009B1 RID: 2481
	public AudioSource source;

	// Token: 0x040009B2 RID: 2482
	public AudioSource fenceSource;
}
