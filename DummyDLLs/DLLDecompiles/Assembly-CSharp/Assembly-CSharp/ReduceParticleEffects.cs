using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class ReduceParticleEffects : MonoBehaviour
{
	// Token: 0x06001626 RID: 5670 RVA: 0x00063368 File Offset: 0x00061568
	private void Start()
	{
		this.gm = GameManager.instance;
		this.gm.RefreshParticleLevel += this.SetEmission;
		this.emitter = base.GetComponent<ParticleSystem>();
		this.emissionRateHigh = ((this.emitter != null) ? this.emitter.emission.rateOverTimeMultiplier : 1f);
		this.emissionRateLow = this.emissionRateHigh / 2f;
		this.maxParticlesHigh = ((this.emitter != null) ? this.emitter.main.maxParticles : 20);
		this.maxParticlesLow = this.maxParticlesHigh / 2;
		this.SetEmission();
		this.init = true;
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x0006342C File Offset: 0x0006162C
	private void SetEmission()
	{
		if (this.emitter != null)
		{
			if (this.gm.gameSettings.particleEffectsLevel == 0)
			{
				this.emitter.emission.rateOverTimeMultiplier = this.emissionRateLow;
				this.emitter.main.maxParticles = this.maxParticlesLow;
				return;
			}
			this.emitter.emission.rateOverTimeMultiplier = this.emissionRateHigh;
			this.emitter.main.maxParticles = this.maxParticlesHigh;
		}
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x000634BE File Offset: 0x000616BE
	private void OnEnable()
	{
		if (this.init)
		{
			this.gm.RefreshParticleLevel += this.SetEmission;
		}
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x000634DF File Offset: 0x000616DF
	private void OnDisable()
	{
		if (this.init)
		{
			this.gm.RefreshParticleLevel -= this.SetEmission;
		}
	}

	// Token: 0x04001490 RID: 5264
	private GameManager gm;

	// Token: 0x04001491 RID: 5265
	private ParticleSystem emitter;

	// Token: 0x04001492 RID: 5266
	private float emissionRateHigh;

	// Token: 0x04001493 RID: 5267
	private float emissionRateLow;

	// Token: 0x04001494 RID: 5268
	private int maxParticlesHigh;

	// Token: 0x04001495 RID: 5269
	private int maxParticlesLow;

	// Token: 0x04001496 RID: 5270
	private bool init;
}
