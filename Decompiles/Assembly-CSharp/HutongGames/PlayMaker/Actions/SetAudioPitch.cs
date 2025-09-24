using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E40 RID: 3648
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioPitch : ComponentAction<AudioSource>
	{
		// Token: 0x06006870 RID: 26736 RVA: 0x0020D26A File Offset: 0x0020B46A
		public override void Reset()
		{
			this.gameObject = null;
			this.pitch = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006871 RID: 26737 RVA: 0x0020D28A File Offset: 0x0020B48A
		public override void OnEnter()
		{
			this.DoSetAudioPitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006872 RID: 26738 RVA: 0x0020D2A0 File Offset: 0x0020B4A0
		public override void OnUpdate()
		{
			this.DoSetAudioPitch();
		}

		// Token: 0x06006873 RID: 26739 RVA: 0x0020D2A8 File Offset: 0x0020B4A8
		private void DoSetAudioPitch()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)) && !this.pitch.IsNone)
			{
				base.audio.pitch = this.pitch.Value;
			}
		}

		// Token: 0x040067A0 RID: 26528
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("A GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067A1 RID: 26529
		[Tooltip("Set the pitch.")]
		public FsmFloat pitch;

		// Token: 0x040067A2 RID: 26530
		[Tooltip("Repeat every frame. Useful if you're driving pitch with a float variable.")]
		public bool everyFrame;
	}
}
