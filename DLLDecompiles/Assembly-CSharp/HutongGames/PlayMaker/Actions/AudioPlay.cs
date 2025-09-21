using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E39 RID: 3641
	[ActionCategory(ActionCategory.Audio)]
	[ActionTarget(typeof(AudioSource), "gameObject", false)]
	[ActionTarget(typeof(AudioClip), "oneShotClip", false)]
	[Tooltip("Plays the AudioClip defined in an Audio Source component on a GameObject. Set the clip using {{Set Audio Clip}}. Optionally plays a one shot Audio Clip.")]
	public class AudioPlay : ComponentAction<AudioSource>
	{
		// Token: 0x06006855 RID: 26709 RVA: 0x0020CBA9 File Offset: 0x0020ADA9
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.oneShotClip = null;
			this.finishedEvent = null;
			this.WaitForEndOfClip = true;
		}

		// Token: 0x06006856 RID: 26710 RVA: 0x0020CBDC File Offset: 0x0020ADDC
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (base.audio == null)
			{
				return;
			}
			AudioClip audioClip = this.oneShotClip.Value as AudioClip;
			if (audioClip == null)
			{
				base.audio.Play();
				if (!this.volume.IsNone)
				{
					base.audio.volume = this.volume.Value;
				}
				if (!this.WaitForEndOfClip.Value)
				{
					base.Fsm.Event(this.finishedEvent);
					base.Finish();
				}
				return;
			}
			if (!this.volume.IsNone)
			{
				base.audio.PlayOneShot(audioClip, this.volume.Value);
			}
			else
			{
				base.audio.PlayOneShot(audioClip);
			}
			if (!this.WaitForEndOfClip.Value)
			{
				base.Fsm.Event(this.finishedEvent);
				base.Finish();
			}
		}

		// Token: 0x06006857 RID: 26711 RVA: 0x0020CCDC File Offset: 0x0020AEDC
		public override void OnUpdate()
		{
			if (base.audio == null)
			{
				base.Finish();
				return;
			}
			if (!base.audio.isPlaying)
			{
				base.Fsm.Event(this.finishedEvent);
				base.Finish();
				return;
			}
			if (!this.volume.IsNone && this.volume.Value != base.audio.volume)
			{
				base.audio.volume = this.volume.Value;
			}
		}

		// Token: 0x04006783 RID: 26499
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006784 RID: 26500
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Volume to play the sound at. Can be modified with {{Set Audio Volume}}.")]
		public FsmFloat volume;

		// Token: 0x04006785 RID: 26501
		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		// Token: 0x04006786 RID: 26502
		[Tooltip("Wait until the end of the clip to send the Finish Event. Set to false to send the finish event immediately.")]
		public FsmBool WaitForEndOfClip;

		// Token: 0x04006787 RID: 26503
		[Tooltip("Send this event when the sound is finished playing. NOTE: currently also sent when the sound is paused...")]
		public FsmEvent finishedEvent;
	}
}
