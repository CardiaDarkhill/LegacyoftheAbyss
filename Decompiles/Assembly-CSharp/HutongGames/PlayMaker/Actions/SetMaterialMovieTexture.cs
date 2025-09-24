using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F68 RID: 3944
	[ActionCategory(ActionCategory.Material)]
	[Obsolete("Use VideoPlayer actions instead.")]
	[Tooltip("Sets a named texture in a game object's material to a movie texture.")]
	public class SetMaterialMovieTexture : ComponentAction<Renderer>
	{
		// Token: 0x06006D6F RID: 28015 RVA: 0x00220768 File Offset: 0x0021E968
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedTexture = "_MainTex";
			this.movieTexture = null;
		}

		// Token: 0x06006D70 RID: 28016 RVA: 0x0022079B File Offset: 0x0021E99B
		public override void OnEnter()
		{
			this.DoSetMaterialTexture();
			base.Finish();
		}

		// Token: 0x06006D71 RID: 28017 RVA: 0x002207A9 File Offset: 0x0021E9A9
		private void DoSetMaterialTexture()
		{
		}

		// Token: 0x06006D72 RID: 28018 RVA: 0x002207AB File Offset: 0x0021E9AB
		public override string ErrorCheck()
		{
			return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
		}

		// Token: 0x04006D33 RID: 27955
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D34 RID: 27956
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x04006D35 RID: 27957
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x04006D36 RID: 27958
		[UIHint(UIHint.NamedTexture)]
		[Tooltip("A named texture in the shader.")]
		public FsmString namedTexture;

		// Token: 0x04006D37 RID: 27959
		[RequiredField]
		[Tooltip("The Movie Texture to use.")]
		public FsmObject movieTexture;
	}
}
