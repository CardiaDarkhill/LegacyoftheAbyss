using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F66 RID: 3942
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets a named color value in a Game Object's material.\n\nNote: With URP and HDRP set NamedColor to _BaseColor instead of _Color")]
	public class SetMaterialColor : ComponentAction<Renderer>
	{
		// Token: 0x06006D65 RID: 28005 RVA: 0x00220450 File Offset: 0x0021E650
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedColor = "_Color";
			this.color = Color.black;
			this.everyFrame = false;
		}

		// Token: 0x06006D66 RID: 28006 RVA: 0x0022049E File Offset: 0x0021E69E
		public override void OnEnter()
		{
			this.DoSetMaterialColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D67 RID: 28007 RVA: 0x002204B4 File Offset: 0x0021E6B4
		public override void OnUpdate()
		{
			this.DoSetMaterialColor();
		}

		// Token: 0x06006D68 RID: 28008 RVA: 0x002204BC File Offset: 0x0021E6BC
		private void DoSetMaterialColor()
		{
			if (this.color.IsNone)
			{
				return;
			}
			string text = this.namedColor.Value;
			if (text == "")
			{
				text = "_Color";
			}
			if (this.material.Value != null)
			{
				this.material.Value.SetColor(text, this.color.Value);
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
				base.renderer.material.SetColor(text, this.color.Value);
				return;
			}
			if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetColor(text, this.color.Value);
				base.renderer.materials = materials;
			}
		}

		// Token: 0x04006D27 RID: 27943
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006D28 RID: 27944
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x04006D29 RID: 27945
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x04006D2A RID: 27946
		[UIHint(UIHint.NamedColor)]
		[Tooltip("The named color.\nNote: With URP and HDRP set NamedColor to _BaseColor instead of _Color.\nSee unity docs: <a href=\"https://docs.unity3d.com/ScriptReference/Material.SetColor.html\" rel=\"nofollow\">Material.SetColor</a>")]
		public FsmString namedColor;

		// Token: 0x04006D2B RID: 27947
		[RequiredField]
		[Tooltip("Set the parameter value.")]
		public FsmColor color;

		// Token: 0x04006D2C RID: 27948
		[Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;
	}
}
