using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBA RID: 3002
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Stops audio when state exited.")]
	public class AudioPlayInStateConditional : FsmStateAction
	{
		// Token: 0x06005C73 RID: 23667 RVA: 0x001D1D08 File Offset: 0x001CFF08
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.activeBool = null;
		}

		// Token: 0x06005C74 RID: 23668 RVA: 0x001D1D28 File Offset: 0x001CFF28
		public override void OnEnter()
		{
			this.didPlay = false;
			if (this.activeBool.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.audio = ownerDefaultTarget.GetComponent<AudioSource>();
					if (this.audio != null)
					{
						if (!this.audio.isPlaying)
						{
							this.audio.Play();
						}
						if (!this.volume.IsNone)
						{
							this.audio.volume = this.volume.Value;
						}
					}
				}
				this.didPlay = true;
			}
		}

		// Token: 0x06005C75 RID: 23669 RVA: 0x001D1DC0 File Offset: 0x001CFFC0
		public override void OnExit()
		{
			if (this.audio != null && this.didPlay)
			{
				this.audio.Stop();
			}
		}

		// Token: 0x040057F0 RID: 22512
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040057F1 RID: 22513
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040057F2 RID: 22514
		public FsmBool activeBool;

		// Token: 0x040057F3 RID: 22515
		private AudioSource audio;

		// Token: 0x040057F4 RID: 22516
		private bool didPlay;
	}
}
