using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F65 RID: 3941
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the material on a Game Object.")]
	public class SetMaterial : ComponentAction<Renderer>
	{
		// Token: 0x06006D61 RID: 28001 RVA: 0x00220385 File Offset: 0x0021E585
		public override void Reset()
		{
			this.gameObject = null;
			this.material = null;
			this.materialIndex = 0;
		}

		// Token: 0x06006D62 RID: 28002 RVA: 0x002203A1 File Offset: 0x0021E5A1
		public override void OnEnter()
		{
			this.DoSetMaterial();
			base.Finish();
		}

		// Token: 0x06006D63 RID: 28003 RVA: 0x002203B0 File Offset: 0x0021E5B0
		private void DoSetMaterial()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.materialIndex.Value == 0)
			{
				base.renderer.material = this.material.Value;
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value] = this.material.Value;
				base.renderer.materials = materials;
			}
		}

		// Token: 0x04006D24 RID: 27940
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("A Game Object with a Renderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D25 RID: 27941
		[Tooltip("The index of the material on the object.")]
		public FsmInt materialIndex;

		// Token: 0x04006D26 RID: 27942
		[RequiredField]
		[Tooltip("The material to apply.")]
		public FsmMaterial material;
	}
}
