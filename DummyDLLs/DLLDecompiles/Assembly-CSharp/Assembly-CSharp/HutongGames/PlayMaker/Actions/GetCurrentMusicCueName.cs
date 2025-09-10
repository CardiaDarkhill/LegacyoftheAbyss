using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB4 RID: 2996
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Get's the name of the currently playing music cue.")]
	public class GetCurrentMusicCueName : FsmStateAction
	{
		// Token: 0x06005C59 RID: 23641 RVA: 0x001D15CE File Offset: 0x001CF7CE
		public override void Reset()
		{
			this.musicCueName = new FsmString
			{
				UseVariable = true
			};
		}

		// Token: 0x06005C5A RID: 23642 RVA: 0x001D15E4 File Offset: 0x001CF7E4
		public override void OnEnter()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				this.musicCueName.Value = "";
			}
			else
			{
				MusicCue currentMusicCue = unsafeInstance.AudioManager.CurrentMusicCue;
				if (currentMusicCue == null)
				{
					this.musicCueName.Value = "";
				}
				else
				{
					this.musicCueName.Value = currentMusicCue.name;
				}
			}
			base.Finish();
		}

		// Token: 0x040057CF RID: 22479
		public FsmString musicCueName;
	}
}
