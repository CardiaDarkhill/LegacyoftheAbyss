using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC1 RID: 3009
	[ActionCategory(ActionCategory.Audio)]
	public class PlayOneShotOnAudioSource : FsmStateAction
	{
		// Token: 0x06005C8F RID: 23695 RVA: 0x001D23F4 File Offset: 0x001D05F4
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClipTable = null;
			this.pitchOffset = 0f;
			this.stopPreviousSound = true;
			this.forcePlay = false;
			this.audio = null;
			this.volumeScale = 1f;
		}

		// Token: 0x06005C90 RID: 23696 RVA: 0x001D2444 File Offset: 0x001D0644
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06005C91 RID: 23697 RVA: 0x001D246C File Offset: 0x001D066C
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
						randomAudioClipTable.PlayOneShotUnsafe(this.audio, this.pitchOffset.Value, this.volumeScale.Value, this.forcePlay);
						return;
					}
					AudioClip audioClip = this.audioClip.Value as AudioClip;
					if (audioClip != null)
					{
						this.audio.PlayOneShot(audioClip, this.volumeScale.Value);
					}
				}
			}
		}

		// Token: 0x0400581E RID: 22558
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400581F RID: 22559
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x04005820 RID: 22560
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject audioClipTable;

		// Token: 0x04005821 RID: 22561
		public FsmFloat pitchOffset;

		// Token: 0x04005822 RID: 22562
		public FsmFloat volumeScale;

		// Token: 0x04005823 RID: 22563
		public bool stopPreviousSound;

		// Token: 0x04005824 RID: 22564
		public bool forcePlay;

		// Token: 0x04005825 RID: 22565
		private GameObject self;

		// Token: 0x04005826 RID: 22566
		private AudioSource audio;
	}
}
