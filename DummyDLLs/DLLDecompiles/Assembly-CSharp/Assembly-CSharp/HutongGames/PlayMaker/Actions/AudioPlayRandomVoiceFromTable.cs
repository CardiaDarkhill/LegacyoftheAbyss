using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBE RID: 3006
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoiceFromTable : FsmStateAction
	{
		// Token: 0x06005C83 RID: 23683 RVA: 0x001D215C File Offset: 0x001D035C
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClipTable = null;
			this.pitchOffset = 0f;
			this.stopPreviousSound = true;
			this.forcePlay = false;
		}

		// Token: 0x06005C84 RID: 23684 RVA: 0x001D218A File Offset: 0x001D038A
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06005C85 RID: 23685 RVA: 0x001D21B4 File Offset: 0x001D03B4
		private void DoPlayRandomClip()
		{
			this.audio = this.self.Value.GetComponent<AudioSource>();
			if (this.audio)
			{
				if (this.stopPreviousSound)
				{
					this.audio.Stop();
				}
				this.audioClipTable.PlayOneShot(this.audio, this.pitchOffset.Value, this.forcePlay);
			}
		}

		// Token: 0x04005808 RID: 22536
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005809 RID: 22537
		public RandomAudioClipTable audioClipTable;

		// Token: 0x0400580A RID: 22538
		public FsmFloat pitchOffset;

		// Token: 0x0400580B RID: 22539
		public bool stopPreviousSound;

		// Token: 0x0400580C RID: 22540
		public bool forcePlay;

		// Token: 0x0400580D RID: 22541
		private FsmGameObject self;

		// Token: 0x0400580E RID: 22542
		private AudioSource audio;
	}
}
