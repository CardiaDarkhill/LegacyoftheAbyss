using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F6C RID: 3948
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the Offset of a named texture in a Game Object's Material. Useful for scrolling texture effects.")]
	public class SetTextureOffset : ComponentAction<Renderer>
	{
		// Token: 0x06006D80 RID: 28032 RVA: 0x00220A94 File Offset: 0x0021EC94
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.offsetX = 0f;
			this.offsetY = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06006D81 RID: 28033 RVA: 0x00220AEB File Offset: 0x0021ECEB
		public override void OnEnter()
		{
			this.DoSetTextureOffset();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D82 RID: 28034 RVA: 0x00220B01 File Offset: 0x0021ED01
		public override void OnUpdate()
		{
			this.DoSetTextureOffset();
		}

		// Token: 0x06006D83 RID: 28035 RVA: 0x00220B0C File Offset: 0x0021ED0C
		private void DoSetTextureOffset()
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
				base.renderer.material.SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
				base.renderer.materials = materials;
			}
		}

		// Token: 0x04006D43 RID: 27971
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The target Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D44 RID: 27972
		[Tooltip("The index of the material on the object.")]
		public FsmInt materialIndex;

		// Token: 0x04006D45 RID: 27973
		[RequiredField]
		[Tooltip("The named texture. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Material.SetTextureOffset.html\" rel=\"nofollow\">SetTextureOffset</a>")]
		[UIHint(UIHint.NamedColor)]
		public FsmString namedTexture;

		// Token: 0x04006D46 RID: 27974
		[RequiredField]
		[Tooltip("The amount to offset in X axis. 1 = full width of texture.")]
		public FsmFloat offsetX;

		// Token: 0x04006D47 RID: 27975
		[RequiredField]
		[Tooltip("The amount to offset in Y axis. 1 = full height of texture.")]
		public FsmFloat offsetY;

		// Token: 0x04006D48 RID: 27976
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
