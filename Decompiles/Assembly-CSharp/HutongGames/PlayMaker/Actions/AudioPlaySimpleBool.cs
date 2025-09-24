using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E45 RID: 3653
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlaySimpleBool : FsmStateAction
	{
		// Token: 0x06006885 RID: 26757 RVA: 0x0020D5CC File Offset: 0x0020B7CC
		public override void Reset()
		{
			this.gameObject = null;
			this.startTime = 0f;
			this.volume = 1f;
			this.oneShotClip = null;
			this.doPlay = true;
		}

		// Token: 0x06006886 RID: 26758 RVA: 0x0020D608 File Offset: 0x0020B808
		public override void OnEnter()
		{
			if (this.doPlay.Value)
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
			}
			base.Finish();
		}

		// Token: 0x040067B1 RID: 26545
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067B2 RID: 26546
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040067B3 RID: 26547
		public FsmFloat startTime;

		// Token: 0x040067B4 RID: 26548
		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		// Token: 0x040067B5 RID: 26549
		public FsmBool doPlay;

		// Token: 0x040067B6 RID: 26550
		private AudioSource audio;
	}
}
