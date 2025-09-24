using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F96 RID: 3990
	[ActionCategory(ActionCategory.Movie)]
	[Obsolete("Use VideoPlayer actions instead.")]
	[Tooltip("Stops playing the Movie Texture, and rewinds it to the beginning.")]
	public class StopMovieTexture : FsmStateAction
	{
		// Token: 0x06006E3D RID: 28221 RVA: 0x002228D3 File Offset: 0x00220AD3
		public override void Reset()
		{
			this.movieTexture = null;
		}

		// Token: 0x06006E3E RID: 28222 RVA: 0x002228DC File Offset: 0x00220ADC
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x06006E3F RID: 28223 RVA: 0x002228E4 File Offset: 0x00220AE4
		public override string ErrorCheck()
		{
			return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
		}

		// Token: 0x04006DE6 RID: 28134
		[RequiredField]
		[Tooltip("The Movie Texture to stop.")]
		public FsmObject movieTexture;
	}
}
