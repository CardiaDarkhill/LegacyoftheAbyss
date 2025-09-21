using System;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
public sealed class CoralCrustTreeBranchStates : AnimatorActivatingStates
{
	// Token: 0x06002C09 RID: 11273 RVA: 0x000C0EE9 File Offset: 0x000BF0E9
	private void OnGrowAnimationEvent()
	{
		this.growParticle.PlayParticles();
		this.jitterSelf.StartTimedJitter();
		this.growAudioEventPlayer.Play();
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x000C0F0C File Offset: 0x000BF10C
	protected override void OnDeactivateWarning()
	{
		base.OnDeactivateWarning();
		this.jitterSelf.StartJitter();
		this.breakAnticParticle.Play(true);
		this.breakAnticAudioGameObject.SetActive(true);
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x000C0F38 File Offset: 0x000BF138
	protected override void OnDeactivate()
	{
		if (Time.frameCount != CoralCrustTreeBranchStates.lastPlayFrame)
		{
			CoralCrustTreeBranchStates.lastPlayFrame = Time.frameCount;
			CoralCrustTreeBranchStates.playCount = 0;
		}
		base.OnDeactivate();
		this.jitterSelf.StopJitter();
		this.breakParticle.PlayParticleSystems();
		this.breakAnticParticle.Stop(true);
		this.breakAnticAudioGameObject.SetActive(false);
		if (CoralCrustTreeBranchStates.playCount < 5 && CameraInfoCache.IsWithinBounds(base.transform.position, Vector2.one))
		{
			this.breakAudioEventPlayer.Play();
			CoralCrustTreeBranchStates.playCount++;
		}
	}

	// Token: 0x04002D63 RID: 11619
	[SerializeField]
	private JitterSelfForTime jitterSelf;

	// Token: 0x04002D64 RID: 11620
	[SerializeField]
	private ParticleSystem breakAnticParticle;

	// Token: 0x04002D65 RID: 11621
	[SerializeField]
	private GameObject breakAnticAudioGameObject;

	// Token: 0x04002D66 RID: 11622
	[SerializeField]
	private PlayParticleEffects breakParticle;

	// Token: 0x04002D67 RID: 11623
	[SerializeField]
	private PlayRandomAudioEvent breakAudioEventPlayer;

	// Token: 0x04002D68 RID: 11624
	[Space]
	[SerializeField]
	private ParticleSystemPool growParticle;

	// Token: 0x04002D69 RID: 11625
	[SerializeField]
	private PlayRandomAudioEvent growAudioEventPlayer;

	// Token: 0x04002D6A RID: 11626
	private static int lastPlayFrame;

	// Token: 0x04002D6B RID: 11627
	private static int playCount;
}
