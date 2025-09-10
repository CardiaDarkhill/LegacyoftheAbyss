using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB7 RID: 2999
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
	public class AudioPlayDelay : FsmStateAction
	{
		// Token: 0x06005C64 RID: 23652 RVA: 0x001D1791 File Offset: 0x001CF991
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.oneShotClip = null;
			this.delay = 1f;
		}

		// Token: 0x06005C65 RID: 23653 RVA: 0x001D17C1 File Offset: 0x001CF9C1
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x06005C66 RID: 23654 RVA: 0x001D17CE File Offset: 0x001CF9CE
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.DoPlay();
			base.Finish();
		}

		// Token: 0x06005C67 RID: 23655 RVA: 0x001D1804 File Offset: 0x001CFA04
		public void DoPlay()
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
							return;
						}
						component.PlayOneShot(audioClip);
						return;
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
		}

		// Token: 0x040057D6 RID: 22486
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040057D7 RID: 22487
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040057D8 RID: 22488
		[ObjectType(typeof(AudioClip))]
		[Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
		public FsmObject oneShotClip;

		// Token: 0x040057D9 RID: 22489
		public FsmFloat delay;

		// Token: 0x040057DA RID: 22490
		private float timer;
	}
}
