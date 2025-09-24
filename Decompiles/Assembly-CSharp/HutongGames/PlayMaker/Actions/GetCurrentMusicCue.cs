using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB3 RID: 2995
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Get's the currently playing music cue.")]
	public class GetCurrentMusicCue : FsmStateAction
	{
		// Token: 0x06005C56 RID: 23638 RVA: 0x001D156B File Offset: 0x001CF76B
		public override void Reset()
		{
			this.musicCue = new FsmObject
			{
				UseVariable = true
			};
		}

		// Token: 0x06005C57 RID: 23639 RVA: 0x001D1580 File Offset: 0x001CF780
		public override void OnEnter()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				this.musicCue.Value = null;
			}
			else
			{
				this.musicCue.Value = unsafeInstance.AudioManager.CurrentMusicCue;
			}
			base.Finish();
		}

		// Token: 0x040057CE RID: 22478
		[ObjectType(typeof(MusicCue))]
		public FsmObject musicCue;
	}
}
