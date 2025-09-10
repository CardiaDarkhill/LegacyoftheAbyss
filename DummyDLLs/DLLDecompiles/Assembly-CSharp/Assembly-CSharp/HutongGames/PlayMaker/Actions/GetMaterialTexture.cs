using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F64 RID: 3940
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Get a texture from a material on a GameObject")]
	public class GetMaterialTexture : ComponentAction<Renderer>
	{
		// Token: 0x06006D5D RID: 27997 RVA: 0x002201CA File Offset: 0x0021E3CA
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.storedTexture = null;
			this.getFromSharedMaterial = false;
		}

		// Token: 0x06006D5E RID: 27998 RVA: 0x002201FD File Offset: 0x0021E3FD
		public override void OnEnter()
		{
			this.DoGetMaterialTexture();
			base.Finish();
		}

		// Token: 0x06006D5F RID: 27999 RVA: 0x0022020C File Offset: 0x0021E40C
		private void DoGetMaterialTexture()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			string text = this.namedTexture.Value;
			if (text == "")
			{
				text = "_MainTex";
			}
			if (this.materialIndex.Value == 0 && !this.getFromSharedMaterial)
			{
				this.storedTexture.Value = base.renderer.material.GetTexture(text);
				return;
			}
			if (this.materialIndex.Value == 0 && this.getFromSharedMaterial)
			{
				this.storedTexture.Value = base.renderer.sharedMaterial.GetTexture(text);
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value && !this.getFromSharedMaterial)
			{
				Material[] materials = base.renderer.materials;
				this.storedTexture.Value = base.renderer.materials[this.materialIndex.Value].GetTexture(text);
				base.renderer.materials = materials;
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value && this.getFromSharedMaterial)
			{
				Material[] sharedMaterials = base.renderer.sharedMaterials;
				this.storedTexture.Value = base.renderer.sharedMaterials[this.materialIndex.Value].GetTexture(text);
				base.renderer.materials = sharedMaterials;
			}
		}

		// Token: 0x04006D1F RID: 27935
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject the Material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D20 RID: 27936
		[Tooltip("The index of the Material in the Materials array.")]
		public FsmInt materialIndex;

		// Token: 0x04006D21 RID: 27937
		[UIHint(UIHint.NamedTexture)]
		[Tooltip("The texture to get. See Unity Shader docs for names.")]
		public FsmString namedTexture;

		// Token: 0x04006D22 RID: 27938
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Title("StoreTexture")]
		[Tooltip("Store the texture in a variable.")]
		public FsmTexture storedTexture;

		// Token: 0x04006D23 RID: 27939
		[Tooltip("Get the shared version of the texture.")]
		public bool getFromSharedMaterial;
	}
}
