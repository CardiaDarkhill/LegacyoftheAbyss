using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F6B RID: 3947
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets a Game Object's material randomly from an array of Materials.")]
	public class SetRandomMaterial : ComponentAction<Renderer>
	{
		// Token: 0x06006D7C RID: 28028 RVA: 0x00220972 File Offset: 0x0021EB72
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.materials = new FsmMaterial[3];
		}

		// Token: 0x06006D7D RID: 28029 RVA: 0x00220993 File Offset: 0x0021EB93
		public override void OnEnter()
		{
			this.DoSetRandomMaterial();
			base.Finish();
		}

		// Token: 0x06006D7E RID: 28030 RVA: 0x002209A4 File Offset: 0x0021EBA4
		private void DoSetRandomMaterial()
		{
			if (this.materials == null)
			{
				return;
			}
			if (this.materials.Length == 0)
			{
				return;
			}
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
				base.renderer.material = this.materials[Random.Range(0, this.materials.Length)].Value;
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] array = base.renderer.materials;
				array[this.materialIndex.Value] = this.materials[Random.Range(0, this.materials.Length)].Value;
				base.renderer.materials = array;
			}
		}

		// Token: 0x04006D40 RID: 27968
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject that the material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D41 RID: 27969
		[Tooltip("The index of the material on the object.")]
		public FsmInt materialIndex;

		// Token: 0x04006D42 RID: 27970
		[Tooltip("Array of materials to randomly select from.")]
		public FsmMaterial[] materials;
	}
}
