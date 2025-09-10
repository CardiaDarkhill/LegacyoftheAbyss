using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E46 RID: 3654
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlaySimpleV2 : FsmStateAction
	{
		// Token: 0x06006888 RID: 26760 RVA: 0x0020D71A File Offset: 0x0020B91A
		public override void Reset()
		{
			this.gameObject = null;
			this.startTime = 0f;
			this.volume = 1f;
			this.oneShotClip = null;
		}

		// Token: 0x06006889 RID: 26761 RVA: 0x0020D74C File Offset: 0x0020B94C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this.audio = ownerDefaultTarget.GetComponent<AudioSource>();
				if (this.audio != null)
				{
					AudioClip audioClip = this.oneShotClip.Value as AudioClip;
					if (audioClip == null)
					{
						if (!this.volume.IsNone)
						{
							this.audio.volume = this.volume.Value;
						}
						if (!this.startTime.IsNone)
						{
							this.audio.time = this.startTime.Value;
						}
						if (!this.audio.isPlaying)
						{
							this.audio.Play();
						}
					}
					else if (!this.volume.IsNone)
					{
						this.audio.PlayOneShot(audioClip, this.volume.Value);
					}
					else
					{
						this.audio.PlayOneShot(audioClip);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040067B7 RID: 26551
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067B8 RID: 26552
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040067B9 RID: 26553
		public FsmFloat startTime;

		// Token: 0x040067BA RID: 26554
		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		// Token: 0x040067BB RID: 26555
		private AudioSource audio;
	}
}
