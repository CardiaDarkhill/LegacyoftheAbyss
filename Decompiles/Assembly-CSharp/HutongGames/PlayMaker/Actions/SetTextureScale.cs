using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F6D RID: 3949
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the Scale of a named texture in a Game Object's Material. Useful for special effects.")]
	public class SetTextureScale : ComponentAction<Renderer>
	{
		// Token: 0x06006D85 RID: 28037 RVA: 0x00220C0C File Offset: 0x0021EE0C
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006D86 RID: 28038 RVA: 0x00220C63 File Offset: 0x0021EE63
		public override void OnEnter()
		{
			this.DoSetTextureScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D87 RID: 28039 RVA: 0x00220C79 File Offset: 0x0021EE79
		public override void OnUpdate()
		{
			this.DoSetTextureScale();
		}

		// Token: 0x06006D88 RID: 28040 RVA: 0x00220C84 File Offset: 0x0021EE84
		private void DoSetTextureScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (base.renderer.material == null)
			{
				base.LogError("Missing Material!");
				return;
			}
			if (this.materialIndex.Value == 0)
			{
				base.renderer.material.SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
				base.renderer.materials = materials;
			}
		}

		// Token: 0x04006D49 RID: 27977
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The target Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D4A RID: 27978
		[Tooltip("The index of the material on the object.")]
		public FsmInt materialIndex;

		// Token: 0x04006D4B RID: 27979
		[UIHint(UIHint.NamedColor)]
		[Tooltip("The named texture. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Material.SetTextureScale.html\" rel=\"nofollow\">SetTextureScale</a>")]
		public FsmString namedTexture;

		// Token: 0x04006D4C RID: 27980
		[RequiredField]
		[Tooltip("Scale in X axis. 2 = double the texture's width.")]
		public FsmFloat scaleX;

		// Token: 0x04006D4D RID: 27981
		[RequiredField]
		[Tooltip("Scale in Y axis. 2 = double the texture's height.")]
		public FsmFloat scaleY;

		// Token: 0x04006D4E RID: 27982
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
