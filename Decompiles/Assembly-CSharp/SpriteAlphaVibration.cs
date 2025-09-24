using System;
using UnityEngine;

// Token: 0x020007A7 RID: 1959
public sealed class SpriteAlphaVibration : ScaledVibration
{
	// Token: 0x06004542 RID: 17730 RVA: 0x0012E91F File Offset: 0x0012CB1F
	private void Awake()
	{
		this.hasSprite = this.spriteRenderer;
		if (!this.hasSprite)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			this.hasSprite = this.spriteRenderer;
			bool flag = this.hasSprite;
		}
	}

	// Token: 0x06004543 RID: 17731 RVA: 0x0012E95E File Offset: 0x0012CB5E
	private void Start()
	{
		if (!this.isPlaying)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06004544 RID: 17732 RVA: 0x0012E96F File Offset: 0x0012CB6F
	private void OnValidate()
	{
		this.hasSprite = this.spriteRenderer;
		if (!this.hasSprite)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			this.hasSprite = this.spriteRenderer;
		}
	}

	// Token: 0x06004545 RID: 17733 RVA: 0x0012E9A7 File Offset: 0x0012CBA7
	private void OnDestroy()
	{
		this.StopVibration();
	}

	// Token: 0x06004546 RID: 17734 RVA: 0x0012E9AF File Offset: 0x0012CBAF
	private void OnDisable()
	{
		this.StopVibration();
	}

	// Token: 0x06004547 RID: 17735 RVA: 0x0012E9B7 File Offset: 0x0012CBB7
	private void LateUpdate()
	{
		this.emission.SetStrength(this.GetStrength());
	}

	// Token: 0x06004548 RID: 17736 RVA: 0x0012E9CC File Offset: 0x0012CBCC
	public override void PlayVibration(float fade = 0f)
	{
		if (this.emission != null && this.emission.IsPlaying)
		{
			return;
		}
		VibrationData vibrationData = this.vibrationDataAsset.VibrationData;
		bool loop = this.loop;
		bool isRealTime = this.isRealTime;
		string tag = this.tag;
		this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, loop, tag, isRealTime);
		base.enabled = (this.isPlaying = (this.emission != null));
		if (this.isPlaying)
		{
			base.FadeInEmission(fade);
			this.emission.SetStrength(this.GetStrength());
		}
	}

	// Token: 0x06004549 RID: 17737 RVA: 0x0012EA5E File Offset: 0x0012CC5E
	public override void StopVibration()
	{
		VibrationEmission emission = this.emission;
		if (emission != null)
		{
			emission.Stop();
		}
		this.emission = null;
		base.enabled = false;
		this.isPlaying = false;
	}

	// Token: 0x0600454A RID: 17738 RVA: 0x0012EA86 File Offset: 0x0012CC86
	private float GetStrength()
	{
		if (!this.hasSprite)
		{
			return 0f;
		}
		return this.spriteRenderer.color.a * this.alphaToStrengthRate * this.internalStrength;
	}

	// Token: 0x04004611 RID: 17937
	[Space]
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04004612 RID: 17938
	[SerializeField]
	private float alphaToStrengthRate = 1f;

	// Token: 0x04004613 RID: 17939
	private bool hasSprite;

	// Token: 0x04004614 RID: 17940
	private bool isPlaying;
}
