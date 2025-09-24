using System;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class DamageTagInfo
{
	// Token: 0x06001278 RID: 4728 RVA: 0x00055DEC File Offset: 0x00053FEC
	public void CheckLerpEmission()
	{
		if (!this.hasLerpEmission && this.SpawnedLoopEffect)
		{
			this.lerpEmission = this.SpawnedLoopEffect.GetComponent<ParticleEffectsLerpEmission>();
			this.hasLerpEmission = this.lerpEmission;
		}
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x00055E28 File Offset: 0x00054028
	public void StopLoopEffect()
	{
		if (!this.SpawnedLoopEffect)
		{
			return;
		}
		ParticleSystem[] componentsInChildren = this.SpawnedLoopEffect.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00055E67 File Offset: 0x00054067
	public void RefreshEmission(float duration)
	{
		if (this.hasLerpEmission)
		{
			this.lerpEmission.Play(duration);
		}
	}

	// Token: 0x04001136 RID: 4406
	public int Stacked;

	// Token: 0x04001137 RID: 4407
	public int HitsLeft;

	// Token: 0x04001138 RID: 4408
	public double NextHitTime;

	// Token: 0x04001139 RID: 4409
	public GameObject SpawnedLoopEffect;

	// Token: 0x0400113A RID: 4410
	public bool RemoveAfterNextHit;

	// Token: 0x0400113B RID: 4411
	public bool hasLerpEmission;

	// Token: 0x0400113C RID: 4412
	public ParticleEffectsLerpEmission lerpEmission;
}
