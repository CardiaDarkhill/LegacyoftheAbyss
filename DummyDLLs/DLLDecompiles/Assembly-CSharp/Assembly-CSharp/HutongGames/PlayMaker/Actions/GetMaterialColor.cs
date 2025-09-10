using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6E RID: 3182
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Gets a named color value from a game object's material.")]
	public class GetMaterialColor : FsmStateAction
	{
		// Token: 0x06006011 RID: 24593 RVA: 0x001E6CEE File Offset: 0x001E4EEE
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedColor = "_Color";
			this.color = null;
			this.fail = null;
		}

		// Token: 0x06006012 RID: 24594 RVA: 0x001E6D28 File Offset: 0x001E4F28
		public override void OnEnter()
		{
			this.DoGetMaterialColor();
			base.Finish();
		}

		// Token: 0x06006013 RID: 24595 RVA: 0x001E6D38 File Offset: 0x001E4F38
		private void DoGetMaterialColor()
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
				if (!this.material.Value.HasProperty(text))
				{
					base.Fsm.Event(this.fail);
					return;
				}
				this.color.Value = this.material.Value.GetColor(text);
				return;
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				if (ownerDefaultTarget.GetComponent<Renderer>() == null)
				{
					base.LogError("Missing Renderer!");
					return;
				}
				if (ownerDefaultTarget.GetComponent<Renderer>().material == null)
				{
					base.LogError("Missing Material!");
					return;
				}
				if (this.materialIndex.Value != 0)
				{
					if (ownerDefaultTarget.GetComponent<Renderer>().materials.Length > this.materialIndex.Value)
					{
						Material[] materials = ownerDefaultTarget.GetComponent<Renderer>().materials;
						if (!materials[this.materialIndex.Value].HasProperty(text))
						{
							base.Fsm.Event(this.fail);
							return;
						}
						this.color.Value = materials[this.materialIndex.Value].GetColor(text);
					}
					return;
				}
				if (!ownerDefaultTarget.GetComponent<Renderer>().material.HasProperty(text))
				{
					base.Fsm.Event(this.fail);
					return;
				}
				this.color.Value = ownerDefaultTarget.GetComponent<Renderer>().material.GetColor(text);
				return;
			}
		}

		// Token: 0x04005D68 RID: 23912
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D69 RID: 23913
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x04005D6A RID: 23914
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x04005D6B RID: 23915
		[UIHint(UIHint.NamedColor)]
		[Tooltip("The named color parameter in the shader.")]
		public FsmString namedColor;

		// Token: 0x04005D6C RID: 23916
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the parameter value.")]
		public FsmColor color;

		// Token: 0x04005D6D RID: 23917
		public FsmEvent fail;
	}
}
