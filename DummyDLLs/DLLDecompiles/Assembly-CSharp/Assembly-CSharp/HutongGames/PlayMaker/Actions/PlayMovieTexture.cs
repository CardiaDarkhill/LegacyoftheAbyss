using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F95 RID: 3989
	[ActionCategory(ActionCategory.Movie)]
	[Obsolete("Use VideoPlayer actions instead.")]
	[Tooltip("Plays a Movie Texture. Use the Movie Texture in a Material, or in the GUI.")]
	public class PlayMovieTexture : FsmStateAction
	{
		// Token: 0x06006E39 RID: 28217 RVA: 0x002228A7 File Offset: 0x00220AA7
		public override void Reset()
		{
			this.movieTexture = null;
			this.loop = false;
		}

		// Token: 0x06006E3A RID: 28218 RVA: 0x002228BC File Offset: 0x00220ABC
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x06006E3B RID: 28219 RVA: 0x002228C4 File Offset: 0x00220AC4
		public override string ErrorCheck()
		{
			return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
		}

		// Token: 0x04006DE4 RID: 28132
		[RequiredField]
		[Tooltip("The movie texture.")]
		public FsmObject movieTexture;

		// Token: 0x04006DE5 RID: 28133
		[Tooltip("Set looping true/false.")]
		public FsmBool loop;
	}
}
