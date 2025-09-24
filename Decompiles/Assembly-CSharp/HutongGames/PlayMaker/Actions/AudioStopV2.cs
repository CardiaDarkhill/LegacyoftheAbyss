using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC3 RID: 3011
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Stops playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioStopV2 : ComponentAction<AudioSource>
	{
		// Token: 0x06005C97 RID: 23703 RVA: 0x001D25D9 File Offset: 0x001D07D9
		public override void Reset()
		{
			this.gameObject = null;
			this.fadeTime = null;
		}

		// Token: 0x06005C98 RID: 23704 RVA: 0x001D25EC File Offset: 0x001D07EC
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.volume = base.audio.volume;
				this.originalClip = base.audio.clip;
				this.hasStopped = false;
				if (this.fadeTime.Value < 0.01f || !base.audio.isPlaying)
				{
					this.hasStopped = true;
					base.audio.Stop();
				}
				else
				{
					this.fadeRoutine = base.StartCoroutine(this.VolumeFade(base.audio, 0f, this.fadeTime.Value));
				}
			}
			base.Finish();
		}

		// Token: 0x06005C99 RID: 23705 RVA: 0x001D269F File Offset: 0x001D089F
		private IEnumerator VolumeFade(AudioSource audioSource, float endVolume, float fadeDuration)
		{
			double startTime = Time.timeAsDouble;
			while (audioSource.isPlaying && Time.timeAsDouble < startTime + (double)fadeDuration)
			{
				float num = (float)(startTime + (double)fadeDuration - (double)Time.time) / fadeDuration;
				num *= num;
				audioSource.volume = num * this.volume + endVolume * (1f - num);
				yield return null;
			}
			if (Math.Abs(endVolume) < 0.01f)
			{
				if (!this.hasStopped && (this.originalClip == null || base.audio.clip == this.originalClip))
				{
					this.hasStopped = true;
					audioSource.Stop();
				}
				this.fadeRoutine = null;
			}
			yield break;
		}

		// Token: 0x06005C9A RID: 23706 RVA: 0x001D26C4 File Offset: 0x001D08C4
		public override void OnExit()
		{
			if (this.fadeRoutine != null)
			{
				base.StopCoroutine(this.fadeRoutine);
				if (!this.cancelOnEarlyExit && base.audio != null && !this.hasStopped && (this.originalClip == null || base.audio.clip == this.originalClip))
				{
					this.hasStopped = true;
					base.audio.Stop();
				}
			}
			this.originalClip = null;
		}

		// Token: 0x04005829 RID: 22569
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400582A RID: 22570
		[Tooltip("Audio Stop can make a hard pop sound. A short fade out can fix this glitch.")]
		public FsmFloat fadeTime;

		// Token: 0x0400582B RID: 22571
		public bool cancelOnEarlyExit;

		// Token: 0x0400582C RID: 22572
		private Coroutine fadeRoutine;

		// Token: 0x0400582D RID: 22573
		private float volume;

		// Token: 0x0400582E RID: 22574
		private AudioClip originalClip;

		// Token: 0x0400582F RID: 22575
		private bool hasStopped;
	}
}
