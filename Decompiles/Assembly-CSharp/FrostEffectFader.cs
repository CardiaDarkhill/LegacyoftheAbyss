using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class FrostEffectFader : MonoBehaviour
{
	// Token: 0x060014B7 RID: 5303 RVA: 0x0005D3E4 File Offset: 0x0005B5E4
	private void OnEnable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		silentInstance.FrostAmountUpdated += this.OnFrostAmountUpdated;
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x0005D414 File Offset: 0x0005B614
	private void OnDisable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		silentInstance.FrostAmountUpdated -= this.OnFrostAmountUpdated;
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x0005D444 File Offset: 0x0005B644
	private void OnFrostAmountUpdated(float frostAmount)
	{
		if (this.lastFrostAmount == frostAmount)
		{
			return;
		}
		this.lastFrostAmount = frostAmount;
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = this.fadeCurve.Evaluate(frostAmount);
		}
		if (this.tk2dSprite)
		{
			this.tk2dSprite.color = this.spriteGradient.Evaluate(frostAmount);
		}
		bool flag = this.isAboveThreshold;
		this.isAboveThreshold = (frostAmount >= this.particleThreshold);
		if (!this.particles)
		{
			return;
		}
		if (this.isAboveThreshold)
		{
			if (!flag)
			{
				this.particles.PlayParticleSystems();
				return;
			}
		}
		else if (flag)
		{
			this.particles.StopParticleSystems();
		}
	}

	// Token: 0x04001316 RID: 4886
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04001317 RID: 4887
	[SerializeField]
	private AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001318 RID: 4888
	[SerializeField]
	private PlayParticleEffects particles;

	// Token: 0x04001319 RID: 4889
	[SerializeField]
	[Range(0f, 1f)]
	private float particleThreshold;

	// Token: 0x0400131A RID: 4890
	[SerializeField]
	private tk2dSprite tk2dSprite;

	// Token: 0x0400131B RID: 4891
	[SerializeField]
	private Gradient spriteGradient;

	// Token: 0x0400131C RID: 4892
	private bool isAboveThreshold;

	// Token: 0x0400131D RID: 4893
	private float lastFrostAmount = -100f;
}
