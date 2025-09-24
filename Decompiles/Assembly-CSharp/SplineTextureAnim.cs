using System;
using System.Collections;
using TeamCherry.SharedUtils;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x0200055E RID: 1374
public class SplineTextureAnim : MonoBehaviour
{
	// Token: 0x06003117 RID: 12567 RVA: 0x000D9EB6 File Offset: 0x000D80B6
	private void Reset()
	{
		this.spline = base.GetComponent<SplineBase>();
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x000D9EC4 File Offset: 0x000D80C4
	private void OnEnable()
	{
		this.spline.UpdateCondition = SplineBase.UpdateConditions.Manual;
		this.animRoutine = base.StartCoroutine(this.Animate());
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x000D9EE4 File Offset: 0x000D80E4
	private void OnDisable()
	{
		base.StopCoroutine(this.animRoutine);
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x000D9EF2 File Offset: 0x000D80F2
	private IEnumerator Animate()
	{
		YieldInstruction wait = (this.fpsLimit > 0f) ? new WaitForSeconds(1f / this.fpsLimit) : null;
		float elapsed = 0f;
		while (elapsed < this.duration)
		{
			float t = elapsed / this.duration;
			float lerpedValue = this.textureOffsetRange.GetLerpedValue(t);
			this.spline.TextureOffset = lerpedValue;
			this.spline.UpdateSpline();
			if (wait == null)
			{
				yield return null;
				elapsed += Time.deltaTime;
			}
			else
			{
				yield return wait;
				elapsed += 1f / this.fpsLimit;
			}
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x04003466 RID: 13414
	[SerializeField]
	private SplineBase spline;

	// Token: 0x04003467 RID: 13415
	[SerializeField]
	private MinMaxFloat textureOffsetRange;

	// Token: 0x04003468 RID: 13416
	[SerializeField]
	private float duration;

	// Token: 0x04003469 RID: 13417
	[SerializeField]
	private float fpsLimit;

	// Token: 0x0400346A RID: 13418
	private Coroutine animRoutine;
}
