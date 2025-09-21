using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
public sealed class LoopedAudioStateBehaviour : StateMachineBehaviour
{
	// Token: 0x060004A4 RID: 1188 RVA: 0x00018EEB File Offset: 0x000170EB
	private void OnDisable()
	{
		if (this.hasAudioSource)
		{
			this.hasAudioSource = false;
			if (this.audioSource != null)
			{
				this.audioSource.Stop();
				this.audioSource = null;
			}
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00018F1C File Offset: 0x0001711C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.hasAudioSource)
		{
			this.hasAudioSource = false;
			if (this.audioSource != null)
			{
				this.audioSource.Stop();
				this.audioSource = null;
			}
		}
		if (this.tryFindAudioSourceOnAnimator)
		{
			this.audioSource = animator.GetComponent<AudioSource>();
			this.hasAudioSource = (this.audioSource != null);
			if (this.hasAudioSource)
			{
				this.audioSource.loop = true;
				this.loopedAudioClip.PlayOnSource(this.audioSource);
				return;
			}
		}
		this.audioSource = this.loopedAudioClip.SpawnAndPlayLooped(this.prefab, animator.transform.position, 0f, new Action(this.OnRecycled));
		this.hasAudioSource = (this.audioSource != null);
		if (this.hasAudioSource)
		{
			return;
		}
		this.audioSource = animator.GetComponent<AudioSource>();
		this.hasAudioSource = (this.audioSource != null);
		if (this.hasAudioSource)
		{
			this.audioSource.loop = true;
			this.loopedAudioClip.PlayOnSource(this.audioSource);
		}
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00019035 File Offset: 0x00017235
	private void OnRecycled()
	{
		this.audioSource = null;
		this.hasAudioSource = false;
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00019045 File Offset: 0x00017245
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.stopWhenAnimationStops && this.hasAudioSource && !animator.IsInTransition(layerIndex) && stateInfo.normalizedTime >= 1f)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00019079 File Offset: 0x00017279
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.Stop();
			this.audioSource = null;
			this.hasAudioSource = false;
		}
	}

	// Token: 0x0400046C RID: 1132
	[SerializeField]
	private AudioEvent loopedAudioClip;

	// Token: 0x0400046D RID: 1133
	[SerializeField]
	private bool tryFindAudioSourceOnAnimator;

	// Token: 0x0400046E RID: 1134
	[Tooltip("Will attempt to find an audio source on animator if null.")]
	[SerializeField]
	private AudioSource prefab;

	// Token: 0x0400046F RID: 1135
	[SerializeField]
	private bool stopWhenAnimationStops;

	// Token: 0x04000470 RID: 1136
	private bool hasAudioSource;

	// Token: 0x04000471 RID: 1137
	private AudioSource audioSource;
}
