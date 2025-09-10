using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBF RID: 3007
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoiceFromTableBool : FsmStateAction
	{
		// Token: 0x06005C87 RID: 23687 RVA: 0x001D2221 File Offset: 0x001D0421
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClipTable = null;
			this.pitchOffset = 0f;
			this.stopPreviousSound = true;
			this.forcePlay = false;
			this.activeBool = null;
		}

		// Token: 0x06005C88 RID: 23688 RVA: 0x001D2256 File Offset: 0x001D0456
		public override void OnEnter()
		{
			if (this.activeBool.Value)
			{
				this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.DoPlayRandomClip();
				base.Finish();
			}
		}

		// Token: 0x06005C89 RID: 23689 RVA: 0x001D2290 File Offset: 0x001D0490
		private void DoPlayRandomClip()
		{
			this.audio = this.self.Value.GetComponent<AudioSource>();
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

		// Token: 0x0400580F RID: 22543
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005810 RID: 22544
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject audioClipTable;

		// Token: 0x04005811 RID: 22545
		public FsmFloat pitchOffset;

		// Token: 0x04005812 RID: 22546
		public bool stopPreviousSound;

		// Token: 0x04005813 RID: 22547
		public bool forcePlay;

		// Token: 0x04005814 RID: 22548
		public FsmBool activeBool;

		// Token: 0x04005815 RID: 22549
		private FsmGameObject self;

		// Token: 0x04005816 RID: 22550
		private AudioSource audio;
	}
}
