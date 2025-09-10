using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F93 RID: 3987
	[Obsolete("Use VideoPlayer actions instead.")]
	[ActionCategory(ActionCategory.Movie)]
	[Tooltip("Sets the Game Object to use to play the audio source associated with a movie texture. Note: the Game Object must have an <a href=\"http://unity3d.com/support/documentation/Components/class-AudioSource.html\">AudioSource</a> component.")]
	public class MovieTextureAudioSettings : FsmStateAction
	{
		// Token: 0x06006E31 RID: 28209 RVA: 0x00222860 File Offset: 0x00220A60
		public override void Reset()
		{
			this.movieTexture = null;
			this.gameObject = null;
		}

		// Token: 0x06006E32 RID: 28210 RVA: 0x00222870 File Offset: 0x00220A70
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x06006E33 RID: 28211 RVA: 0x00222878 File Offset: 0x00220A78
		public override string ErrorCheck()
		{
			return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
		}

		// Token: 0x04006DE1 RID: 28129
		[RequiredField]
		[Tooltip("The movie texture to set.")]
		public FsmObject movieTexture;

		// Token: 0x04006DE2 RID: 28130
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The Game Object to use to play audio. Should have an AudioSource component.")]
		public FsmGameObject gameObject;
	}
}
