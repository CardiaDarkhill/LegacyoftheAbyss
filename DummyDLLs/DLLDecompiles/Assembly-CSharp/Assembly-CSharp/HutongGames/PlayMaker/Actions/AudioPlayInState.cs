using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E43 RID: 3651
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Stops audio when state exited.")]
	public class AudioPlayInState : FsmStateAction
	{
		// Token: 0x0600687E RID: 26750 RVA: 0x0020D443 File Offset: 0x0020B643
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
		}

		// Token: 0x0600687F RID: 26751 RVA: 0x0020D45C File Offset: 0x0020B65C
		public override void OnEnter()
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
		}

		// Token: 0x06006880 RID: 26752 RVA: 0x0020D4D9 File Offset: 0x0020B6D9
		public override void OnExit()
		{
			if (this.audio != null)
			{
				this.audio.Stop();
			}
		}

		// Token: 0x040067AB RID: 26539
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067AC RID: 26540
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040067AD RID: 26541
		private AudioSource audio;
	}
}
