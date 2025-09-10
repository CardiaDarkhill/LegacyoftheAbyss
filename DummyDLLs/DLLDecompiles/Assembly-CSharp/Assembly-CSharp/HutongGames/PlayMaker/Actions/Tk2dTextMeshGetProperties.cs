using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B87 RID: 2951
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the textMesh properties in one go just for convenience. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetProperties : FsmStateAction
	{
		// Token: 0x06005B6D RID: 23405 RVA: 0x001CCC88 File Offset: 0x001CAE88
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B6E RID: 23406 RVA: 0x001CCCC0 File Offset: 0x001CAEC0
		public override void Reset()
		{
			this.gameObject = null;
			this.text = null;
			this.inlineStyling = null;
			this.textureGradient = null;
			this.mainColor = null;
			this.gradientColor = null;
			this.useGradient = null;
			this.anchor = null;
			this.scale = null;
			this.kerning = null;
			this.maxChars = null;
			this.NumDrawnCharacters = null;
		}

		// Token: 0x06005B6F RID: 23407 RVA: 0x001CCD21 File Offset: 0x001CAF21
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetProperties();
			base.Finish();
		}

		// Token: 0x06005B70 RID: 23408 RVA: 0x001CCD38 File Offset: 0x001CAF38
		private void DoGetProperties()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			this.text.Value = this._textMesh.text;
			this.inlineStyling.Value = this._textMesh.inlineStyling;
			this.textureGradient.Value = this._textMesh.textureGradient;
			this.mainColor.Value = this._textMesh.color;
			this.gradientColor.Value = this._textMesh.color2;
			this.useGradient.Value = this._textMesh.useGradient;
			this.anchor.Value = this._textMesh.anchor.ToString();
			this.scale.Value = this._textMesh.scale;
			this.kerning.Value = this._textMesh.kerning;
			this.maxChars.Value = this._textMesh.maxChars;
			this.NumDrawnCharacters.Value = this._textMesh.NumDrawnCharacters();
			this.textureGradient.Value = this._textMesh.textureGradient;
		}

		// Token: 0x040056D9 RID: 22233
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056DA RID: 22234
		[Tooltip("The Text")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		// Token: 0x040056DB RID: 22235
		[Tooltip("InlineStyling")]
		[UIHint(UIHint.Variable)]
		public FsmBool inlineStyling;

		// Token: 0x040056DC RID: 22236
		[Tooltip("Anchor")]
		[UIHint(UIHint.Variable)]
		public FsmString anchor;

		// Token: 0x040056DD RID: 22237
		[Tooltip("Kerning")]
		[UIHint(UIHint.Variable)]
		public FsmBool kerning;

		// Token: 0x040056DE RID: 22238
		[Tooltip("maxChars")]
		[UIHint(UIHint.Variable)]
		public FsmInt maxChars;

		// Token: 0x040056DF RID: 22239
		[Tooltip("number of drawn characters")]
		[UIHint(UIHint.Variable)]
		public FsmInt NumDrawnCharacters;

		// Token: 0x040056E0 RID: 22240
		[Tooltip("The Main Color")]
		[UIHint(UIHint.Variable)]
		public FsmColor mainColor;

		// Token: 0x040056E1 RID: 22241
		[Tooltip("The Gradient Color. Only used if gradient is true")]
		[UIHint(UIHint.Variable)]
		public FsmColor gradientColor;

		// Token: 0x040056E2 RID: 22242
		[Tooltip("Use gradient")]
		[UIHint(UIHint.Variable)]
		public FsmBool useGradient;

		// Token: 0x040056E3 RID: 22243
		[Tooltip("Texture gradient")]
		[UIHint(UIHint.Variable)]
		public FsmInt textureGradient;

		// Token: 0x040056E4 RID: 22244
		[Tooltip("Scale")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;

		// Token: 0x040056E5 RID: 22245
		private tk2dTextMesh _textMesh;
	}
}
