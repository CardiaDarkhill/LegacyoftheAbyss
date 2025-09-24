using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E3A RID: 3642
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Stops playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioStop : ComponentAction<AudioSource>
	{
		// Token: 0x06006859 RID: 26713 RVA: 0x0020CD66 File Offset: 0x0020AF66
		public override void Reset()
		{
			this.gameObject = null;
			this.fadeTime = null;
		}

		// Token: 0x0600685A RID: 26714 RVA: 0x0020CD78 File Offset: 0x0020AF78
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.volume = base.audio.volume;
				this.originalClip = base.audio.clip;
				if (this.fadeTime.Value < 0.01f || !base.audio.isPlaying)
				{
					base.audio.Stop();
				}
				else
				{
					base.StartCoroutine(this.VolumeFade(base.audio, 0f, this.fadeTime.Value));
				}
			}
			base.Finish();
		}

		// Token: 0x0600685B RID: 26715 RVA: 0x0020CE15 File Offset: 0x0020B015
		public override void OnExit()
		{
			base.OnExit();
			this.originalClip = null;
		}

		// Token: 0x0600685C RID: 26716 RVA: 0x0020CE24 File Offset: 0x0020B024
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
			if (Math.Abs(endVolume) < 0.01f && (this.originalClip == null || base.audio.clip == this.originalClip))
			{
				audioSource.Stop();
			}
			yield break;
		}

		// Token: 0x04006788 RID: 26504
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006789 RID: 26505
		[Tooltip("Audio Stop can make a hard pop sound. A short fade out can fix this glitch.")]
		public FsmFloat fadeTime;

		// Token: 0x0400678A RID: 26506
		private float volume;

		// Token: 0x0400678B RID: 26507
		private AudioClip originalClip;
	}
}
