using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E44 RID: 3652
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlaySimple : FsmStateAction
	{
		// Token: 0x06006882 RID: 26754 RVA: 0x0020D4FC File Offset: 0x0020B6FC
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.oneShotClip = null;
		}

		// Token: 0x06006883 RID: 26755 RVA: 0x0020D51C File Offset: 0x0020B71C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					AudioClip audioClip = this.oneShotClip.Value as AudioClip;
					if (audioClip != null)
					{
						if (!this.volume.IsNone)
						{
							component.PlayOneShot(audioClip, this.volume.Value);
						}
						else
						{
							component.PlayOneShot(audioClip);
						}
					}
					else
					{
						if (!component.isPlaying)
						{
							component.Play();
						}
						if (!this.volume.IsNone)
						{
							component.volume = this.volume.Value;
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040067AE RID: 26542
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067AF RID: 26543
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040067B0 RID: 26544
		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;
	}
}
