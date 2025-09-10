using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F94 RID: 3988
	[ActionCategory(ActionCategory.Movie)]
	[Obsolete("Use VideoPlayer actions instead.")]
	[Tooltip("Pauses a Movie Texture.")]
	public class PauseMovieTexture : FsmStateAction
	{
		// Token: 0x06006E35 RID: 28213 RVA: 0x00222887 File Offset: 0x00220A87
		public override void Reset()
		{
			this.movieTexture = null;
		}

		// Token: 0x06006E36 RID: 28214 RVA: 0x00222890 File Offset: 0x00220A90
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x06006E37 RID: 28215 RVA: 0x00222898 File Offset: 0x00220A98
		public override string ErrorCheck()
		{
			return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
		}

		// Token: 0x04006DE3 RID: 28131
		[RequiredField]
		[Tooltip("The Movie Texture to pause.")]
		public FsmObject movieTexture;
	}
}
