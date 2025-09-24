using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class RandomLooptk2dAnimator : MonoBehaviour
{
	// Token: 0x060004FF RID: 1279 RVA: 0x0001A027 File Offset: 0x00018227
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001A035 File Offset: 0x00018235
	private void OnEnable()
	{
		if (this.animator)
		{
			base.StartCoroutine(this.Animate());
		}
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001A051 File Offset: 0x00018251
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001A059 File Offset: 0x00018259
	private IEnumerator Animate()
	{
		this.animator.Play(this.defaultAnim);
		yield return new WaitForSeconds(Random.Range(this.minDelay, this.maxDelay));
		for (;;)
		{
			yield return base.StartCoroutine(this.animator.PlayAnimWait(this.defaultAnim, null));
			yield return new WaitForSeconds(Random.Range(this.minDelay, this.maxDelay));
			yield return base.StartCoroutine(this.animator.PlayAnimWait(this.variantAnim, null));
		}
		yield break;
	}

	// Token: 0x040004D8 RID: 1240
	[SerializeField]
	private string defaultAnim;

	// Token: 0x040004D9 RID: 1241
	[SerializeField]
	private string variantAnim;

	// Token: 0x040004DA RID: 1242
	[SerializeField]
	private float minDelay = 1f;

	// Token: 0x040004DB RID: 1243
	[SerializeField]
	private float maxDelay = 2f;

	// Token: 0x040004DC RID: 1244
	private tk2dSpriteAnimator animator;
}
