using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class FlashAnimator : BaseAnimator
{
	// Token: 0x060003BD RID: 957 RVA: 0x00012B74 File Offset: 0x00010D74
	private void Awake()
	{
		this.block = new MaterialPropertyBlock();
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00012B84 File Offset: 0x00010D84
	public override void StartAnimation()
	{
		if (!this.renderer)
		{
			return;
		}
		if (this.flashRoutine != null)
		{
			base.StopCoroutine(this.flashRoutine);
			this.ResetFlash();
		}
		this.SetRendererProperty("_FlashColor", this.flashColour);
		this.flashRoutine = this.StartTimerRoutine(0f, this.duration, delegate(float time)
		{
			this.SetRendererProperty("_FlashAmount", Mathf.Lerp(0f, this.maxAmount, this.curve.Evaluate(time)));
		}, null, new Action(this.ResetFlash), false);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00012BFB File Offset: 0x00010DFB
	private void ResetFlash()
	{
		this.SetRendererProperty("_FlashAmount", 0f);
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x00012C0D File Offset: 0x00010E0D
	private void SetRendererProperty(string property, float value)
	{
		this.renderer.GetPropertyBlock(this.block);
		this.block.SetFloat(property, value);
		this.renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00012C3E File Offset: 0x00010E3E
	private void SetRendererProperty(string property, Color value)
	{
		this.renderer.GetPropertyBlock(this.block);
		this.block.SetColor(property, value);
		this.renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x0400035B RID: 859
	[SerializeField]
	private Renderer renderer;

	// Token: 0x0400035C RID: 860
	private MaterialPropertyBlock block;

	// Token: 0x0400035D RID: 861
	[SerializeField]
	private float maxAmount = 1f;

	// Token: 0x0400035E RID: 862
	[SerializeField]
	private Color flashColour = Color.white;

	// Token: 0x0400035F RID: 863
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04000360 RID: 864
	[SerializeField]
	private float duration = 0.2f;

	// Token: 0x04000361 RID: 865
	private Coroutine flashRoutine;
}
