using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E3D RID: 3645
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioClip : ComponentAction<AudioSource>
	{
		// Token: 0x06006866 RID: 26726 RVA: 0x0020D0F1 File Offset: 0x0020B2F1
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClip = null;
			this.autoPlay = null;
			this.stopOnExit = null;
		}

		// Token: 0x06006867 RID: 26727 RVA: 0x0020D110 File Offset: 0x0020B310
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				AudioClip clip = base.audio.clip;
				base.audio.clip = (this.audioClip.Value as AudioClip);
				if (this.autoPlay.Value)
				{
					if (base.audio.clip != clip)
					{
						base.audio.Stop();
						base.audio.time = 0f;
					}
					base.audio.Play();
				}
			}
			base.Finish();
		}

		// Token: 0x06006868 RID: 26728 RVA: 0x0020D1AB File Offset: 0x0020B3AB
		public override void OnExit()
		{
			if (this.stopOnExit.Value && this.autoPlay.Value)
			{
				base.audio.Stop();
			}
			base.OnExit();
		}

		// Token: 0x04006798 RID: 26520
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006799 RID: 26521
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to set.")]
		public FsmObject audioClip;

		// Token: 0x0400679A RID: 26522
		public FsmBool autoPlay;

		// Token: 0x0400679B RID: 26523
		public FsmBool stopOnExit;
	}
}
