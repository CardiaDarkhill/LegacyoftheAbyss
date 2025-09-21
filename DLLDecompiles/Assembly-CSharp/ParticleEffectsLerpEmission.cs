using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class ParticleEffectsLerpEmission : MonoBehaviour
{
	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001591 RID: 5521 RVA: 0x0006185D File Offset: 0x0005FA5D
	// (set) Token: 0x06001592 RID: 5522 RVA: 0x00061865 File Offset: 0x0005FA65
	public float TotalMultiplier { get; set; }

	// Token: 0x06001593 RID: 5523 RVA: 0x0006186E File Offset: 0x0005FA6E
	private void Awake()
	{
		this.TotalMultiplier = 1f;
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0006187C File Offset: 0x0005FA7C
	private void OnEnable()
	{
		if (this.initialMultipliers == null || this.initialMultipliers.Length != this.particles.Length)
		{
			this.initialMultipliers = new float[this.particles.Length];
		}
		for (int i = 0; i < this.initialMultipliers.Length; i++)
		{
			ParticleSystem.EmissionModule emission = this.particles[i].emission;
			this.initialMultipliers[i] = emission.rateOverTimeMultiplier;
		}
		this.SetEmissionMultiplier(1f);
		this.hasStarted = false;
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000618F8 File Offset: 0x0005FAF8
	private void OnDisable()
	{
		for (int i = 0; i < this.initialMultipliers.Length; i++)
		{
			this.particles[i].emission.rateOverTimeMultiplier = this.initialMultipliers[i];
		}
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x00061938 File Offset: 0x0005FB38
	private void Update()
	{
		if (!this.hasStarted)
		{
			return;
		}
		if (this.ShouldRecycle())
		{
			base.gameObject.Recycle();
			return;
		}
		if (this.elapsedDuration < this.emissionDuration)
		{
			this.elapsedDuration += Time.deltaTime;
			float num = this.elapsedDuration / this.emissionDuration;
			num = Mathf.Clamp01(num);
			this.SetEmissionMultiplier(num);
			if (num >= 1f)
			{
				this.Stop();
			}
		}
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x000619AC File Offset: 0x0005FBAC
	private bool ShouldRecycle()
	{
		if (!this.recycleOnEnd)
		{
			return false;
		}
		ParticleSystem[] array = this.particles;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive(true))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x000619E8 File Offset: 0x0005FBE8
	public void Play(float duration)
	{
		ParticleSystem[] array = this.particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(true);
		}
		this.SetEmissionMultiplier(0f);
		this.hasStarted = true;
		this.emissionDuration = duration;
		this.elapsedDuration = 0f;
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x00061A38 File Offset: 0x0005FC38
	public void Stop()
	{
		ParticleSystem[] array = this.particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true);
		}
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x00061A64 File Offset: 0x0005FC64
	public void SetEmissionMultiplier(float t)
	{
		t = this.emissionMultiplierCurve.Evaluate(t);
		for (int i = 0; i < this.particles.Length; i++)
		{
			this.particles[i].emission.rateOverTimeMultiplier = this.initialMultipliers[i] * this.emissionMultiplier.GetLerpedValue(t) * this.TotalMultiplier;
		}
	}

	// Token: 0x04001436 RID: 5174
	[SerializeField]
	private ParticleSystem[] particles;

	// Token: 0x04001437 RID: 5175
	[Space]
	[SerializeField]
	private AnimationCurve emissionMultiplierCurve;

	// Token: 0x04001438 RID: 5176
	[SerializeField]
	private MinMaxFloat emissionMultiplier;

	// Token: 0x04001439 RID: 5177
	[SerializeField]
	private bool recycleOnEnd = true;

	// Token: 0x0400143A RID: 5178
	private float[] initialMultipliers;

	// Token: 0x0400143B RID: 5179
	private bool hasStarted;

	// Token: 0x0400143C RID: 5180
	private float emissionDuration;

	// Token: 0x0400143D RID: 5181
	private float elapsedDuration;
}
