using System;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB5 RID: 2997
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Get's the name of the currently playing music cue.")]
	public sealed class ApplyMusicSnapshot : FsmStateAction
	{
		// Token: 0x06005C5C RID: 23644 RVA: 0x001D1658 File Offset: 0x001CF858
		public override void Reset()
		{
			this.musicSnapshot = null;
			this.delayTime = null;
			this.transitionTime = null;
			this.skipMusicMarkerBlock = null;
		}

		// Token: 0x06005C5D RID: 23645 RVA: 0x001D1678 File Offset: 0x001CF878
		public override void OnEnter()
		{
			AudioMixerSnapshot audioMixerSnapshot = this.musicSnapshot.Value as AudioMixerSnapshot;
			GameManager instance = GameManager.instance;
			if (!(audioMixerSnapshot == null) && !(instance == null))
			{
				instance.AudioManager.ApplyMusicSnapshot(audioMixerSnapshot, this.delayTime.Value, this.transitionTime.Value, !this.skipMusicMarkerBlock.Value);
			}
			base.Finish();
		}

		// Token: 0x040057D0 RID: 22480
		[RequiredField]
		[ObjectType(typeof(AudioMixerSnapshot))]
		public FsmObject musicSnapshot;

		// Token: 0x040057D1 RID: 22481
		public FsmFloat delayTime;

		// Token: 0x040057D2 RID: 22482
		public FsmFloat transitionTime;

		// Token: 0x040057D3 RID: 22483
		public FsmBool skipMusicMarkerBlock;
	}
}
