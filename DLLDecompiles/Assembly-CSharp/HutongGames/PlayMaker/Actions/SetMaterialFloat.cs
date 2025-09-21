using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F67 RID: 3943
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets a named float in a game object's material.")]
	public class SetMaterialFloat : ComponentAction<Renderer>
	{
		// Token: 0x06006D6A RID: 28010 RVA: 0x002205E4 File Offset: 0x0021E7E4
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedFloat = "";
			this.floatValue = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06006D6B RID: 28011 RVA: 0x00220632 File Offset: 0x0021E832
		public override void OnEnter()
		{
			this.DoSetMaterialFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D6C RID: 28012 RVA: 0x00220648 File Offset: 0x0021E848
		public override void OnUpdate()
		{
			this.DoSetMaterialFloat();
		}

		// Token: 0x06006D6D RID: 28013 RVA: 0x00220650 File Offset: 0x0021E850
		private void DoSetMaterialFloat()
		{
			if (this.material.Value != null)
			{
				this.material.Value.SetFloat(this.namedFloat.Value, this.floatValue.Value);
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
				base.renderer.material.SetFloat(this.namedFloat.Value, this.floatValue.Value);
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetFloat(this.namedFloat.Value, this.floatValue.Value);
				base.renderer.materials = materials;
			}
		}

		// Token: 0x04006D2D RID: 27949
		[Tooltip("The Game Object that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D2E RID: 27950
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x04006D2F RID: 27951
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x04006D30 RID: 27952
		[RequiredField]
		[Tooltip("A named float parameter in the shader.")]
		public FsmString namedFloat;

		// Token: 0x04006D31 RID: 27953
		[RequiredField]
		[Tooltip("Set the parameter value.")]
		public FsmFloat floatValue;

		// Token: 0x04006D32 RID: 27954
		[Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;
	}
}
