using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000549 RID: 1353
public class ScaleActivation : MonoBehaviour
{
	// Token: 0x06003060 RID: 12384 RVA: 0x000D5999 File Offset: 0x000D3B99
	private void Awake()
	{
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x000D59AC File Offset: 0x000D3BAC
	public void Activate()
	{
		base.gameObject.SetActive(true);
		base.transform.localScale = this.initialScale.MultiplyElements(this.fromScale);
		base.transform.ScaleTo(this, this.initialScale.MultiplyElements(this.toScale), this.scaleUpDuration, this.startDelay.GetRandomValue(), false, false, null);
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x000D5A14 File Offset: 0x000D3C14
	public void Deactivate()
	{
		base.transform.ScaleTo(this, this.initialScale.MultiplyElements(this.fromScale), this.scaleDownDuration, this.endDelay.GetRandomValue(), false, false, delegate
		{
			base.gameObject.SetActive(false);
		});
	}

	// Token: 0x04003346 RID: 13126
	[SerializeField]
	private Vector3 fromScale = Vector3.zero;

	// Token: 0x04003347 RID: 13127
	[SerializeField]
	private Vector3 toScale = Vector3.one;

	// Token: 0x04003348 RID: 13128
	[SerializeField]
	private MinMaxFloat startDelay;

	// Token: 0x04003349 RID: 13129
	[SerializeField]
	private MinMaxFloat endDelay;

	// Token: 0x0400334A RID: 13130
	[SerializeField]
	private float scaleUpDuration;

	// Token: 0x0400334B RID: 13131
	[SerializeField]
	private float scaleDownDuration;

	// Token: 0x0400334C RID: 13132
	private Vector3 initialScale;
}
