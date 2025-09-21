using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007A8 RID: 1960
public abstract class ScaledVibration : MonoBehaviour
{
	// Token: 0x0600454C RID: 17740
	[ContextMenu("Play Vibration")]
	public abstract void PlayVibration(float fade = 0f);

	// Token: 0x0600454D RID: 17741
	public abstract void StopVibration();

	// Token: 0x0600454E RID: 17742 RVA: 0x0012EAC8 File Offset: 0x0012CCC8
	protected void FadeInEmission(float duration)
	{
		if (duration <= 0f)
		{
			return;
		}
		if (this.emission == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.emission.SetStrength(0f);
		this.internalStrength = 0f;
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(1f, duration));
	}

	// Token: 0x0600454F RID: 17743 RVA: 0x0012EB3C File Offset: 0x0012CD3C
	public void FadeOut(float duration)
	{
		if (duration <= 0f)
		{
			return;
		}
		if (this.emission == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			this.StopVibration();
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(0f, duration));
	}

	// Token: 0x06004550 RID: 17744 RVA: 0x0012EB9B File Offset: 0x0012CD9B
	private IEnumerator FadeRoutine(float targetStrength, float fade)
	{
		float inverse = 1f / fade;
		float start = this.internalStrength;
		float t = 0f;
		while (t < 1f)
		{
			yield return null;
			float deltaTime = Time.deltaTime;
			t += deltaTime * inverse;
			this.internalStrength = Mathf.Lerp(start, targetStrength, t);
		}
		this.internalStrength = targetStrength;
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x04004615 RID: 17941
	[SerializeField]
	protected VibrationDataAsset vibrationDataAsset;

	// Token: 0x04004616 RID: 17942
	[SerializeField]
	protected bool loop = true;

	// Token: 0x04004617 RID: 17943
	[SerializeField]
	protected bool isRealTime;

	// Token: 0x04004618 RID: 17944
	[SerializeField]
	protected new string tag;

	// Token: 0x04004619 RID: 17945
	protected VibrationEmission emission;

	// Token: 0x0400461A RID: 17946
	protected Coroutine fadeRoutine;

	// Token: 0x0400461B RID: 17947
	protected float internalStrength;
}
