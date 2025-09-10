using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC0 RID: 3008
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoiceFromTableV2 : FsmStateAction
	{
		// Token: 0x06005C8B RID: 23691 RVA: 0x001D2312 File Offset: 0x001D0512
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClipTable = null;
			this.pitchOffset = 0f;
			this.stopPreviousSound = true;
			this.forcePlay = false;
		}

		// Token: 0x06005C8C RID: 23692 RVA: 0x001D2340 File Offset: 0x001D0540
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06005C8D RID: 23693 RVA: 0x001D2368 File Offset: 0x001D0568
		private void DoPlayRandomClip()
		{
			if (this.self != null)
			{
				this.audio = this.self.GetComponent<AudioSource>();
				if (this.audio)
				{
					if (this.stopPreviousSound)
					{
						this.audio.Stop();
					}
					RandomAudioClipTable randomAudioClipTable = this.audioClipTable.Value as RandomAudioClipTable;
					if (randomAudioClipTable != null)
					{
						randomAudioClipTable.PlayOneShot(this.audio, this.pitchOffset.Value, this.forcePlay);
					}
				}
			}
		}

		// Token: 0x04005817 RID: 22551
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005818 RID: 22552
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject audioClipTable;

		// Token: 0x04005819 RID: 22553
		public FsmFloat pitchOffset;

		// Token: 0x0400581A RID: 22554
		public bool stopPreviousSound;

		// Token: 0x0400581B RID: 22555
		public bool forcePlay;

		// Token: 0x0400581C RID: 22556
		private GameObject self;

		// Token: 0x0400581D RID: 22557
		private AudioSource audio;
	}
}
