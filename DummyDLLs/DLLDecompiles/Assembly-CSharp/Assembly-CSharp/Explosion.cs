using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class Explosion : MonoBehaviour
{
	// Token: 0x060000B2 RID: 178 RVA: 0x000052B4 File Offset: 0x000034B4
	private void OnEnable()
	{
		base.StartCoroutine(this.Shrink());
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x000052C3 File Offset: 0x000034C3
	private IEnumerator Shrink()
	{
		base.transform.localScale = Vector3.one;
		float elapsed = 0f;
		while (elapsed < this.duration)
		{
			float num = 1f - this.animationCurve.Evaluate(elapsed / this.duration);
			base.transform.localScale = new Vector3(num, num, num);
			elapsed += Time.deltaTime;
			yield return 0;
		}
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x040000A0 RID: 160
	public AnimationCurve animationCurve;

	// Token: 0x040000A1 RID: 161
	public float duration;
}
