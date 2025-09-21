using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F63 RID: 3939
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Get a material at index on a gameObject and store it in a variable")]
	public class GetMaterial : ComponentAction<Renderer>
	{
		// Token: 0x06006D59 RID: 27993 RVA: 0x00220069 File Offset: 0x0021E269
		public override void Reset()
		{
			this.gameObject = null;
			this.material = null;
			this.materialIndex = 0;
			this.getSharedMaterial = false;
		}

		// Token: 0x06006D5A RID: 27994 RVA: 0x0022008C File Offset: 0x0021E28C
		public override void OnEnter()
		{
			this.DoGetMaterial();
			base.Finish();
		}

		// Token: 0x06006D5B RID: 27995 RVA: 0x0022009C File Offset: 0x0021E29C
		private void DoGetMaterial()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.materialIndex.Value == 0 && !this.getSharedMaterial)
			{
				this.material.Value = base.renderer.material;
				return;
			}
			if (this.materialIndex.Value == 0 && this.getSharedMaterial)
			{
				this.material.Value = base.renderer.sharedMaterial;
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value && !this.getSharedMaterial)
			{
				Material[] materials = base.renderer.materials;
				this.material.Value = materials[this.materialIndex.Value];
				base.renderer.materials = materials;
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value && this.getSharedMaterial)
			{
				Material[] sharedMaterials = base.renderer.sharedMaterials;
				this.material.Value = sharedMaterials[this.materialIndex.Value];
				base.renderer.sharedMaterials = sharedMaterials;
			}
		}

		// Token: 0x04006D1B RID: 27931
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject the Material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D1C RID: 27932
		[Tooltip("The index of the Material in the Materials array.")]
		public FsmInt materialIndex;

		// Token: 0x04006D1D RID: 27933
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the material in a variable.")]
		public FsmMaterial material;

		// Token: 0x04006D1E RID: 27934
		[Tooltip("Get the shared material of this object. NOTE: Modifying the shared material will change the appearance of all objects using this material, and change material settings that are stored in the project too.")]
		public bool getSharedMaterial;
	}
}
