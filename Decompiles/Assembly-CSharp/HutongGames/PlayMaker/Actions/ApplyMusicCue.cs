using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB2 RID: 2994
	[ActionCategory(ActionCategory.Audio)]
	[ActionTarget(typeof(MusicCue), "musicCue", false)]
	[Tooltip("Plays music cues.")]
	public class ApplyMusicCue : FsmStateAction
	{
		// Token: 0x06005C52 RID: 23634 RVA: 0x001D14A8 File Offset: 0x001CF6A8
		public override void Awake()
		{
			MusicCue musicCue = this.musicCue.Value as MusicCue;
			if (musicCue)
			{
				musicCue.Preload(base.Owner);
			}
		}

		// Token: 0x06005C53 RID: 23635 RVA: 0x001D14DA File Offset: 0x001CF6DA
		public override void Reset()
		{
			this.musicCue = null;
			this.delayTime = 0f;
			this.transitionTime = 0f;
		}

		// Token: 0x06005C54 RID: 23636 RVA: 0x001D1504 File Offset: 0x001CF704
		public override void OnEnter()
		{
			MusicCue x = this.musicCue.Value as MusicCue;
			GameManager instance = GameManager.instance;
			if (!(x == null) && !(instance == null))
			{
				instance.AudioManager.ApplyMusicCue(x, this.delayTime.Value, this.transitionTime.Value, false);
			}
			base.Finish();
		}

		// Token: 0x040057CB RID: 22475
		[Tooltip("Music cue to play.")]
		[ObjectType(typeof(MusicCue))]
		public FsmObject musicCue;

		// Token: 0x040057CC RID: 22476
		[Tooltip("Delay before starting transition")]
		public FsmFloat delayTime;

		// Token: 0x040057CD RID: 22477
		[Tooltip("Transition duration.")]
		public FsmFloat transitionTime;
	}
}
