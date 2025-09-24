using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B92 RID: 2962
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the textMesh properties in one go just for convenience. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetProperties : FsmStateAction
	{
		// Token: 0x06005BAC RID: 23468 RVA: 0x001CD9B8 File Offset: 0x001CBBB8
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005BAD RID: 23469 RVA: 0x001CD9F0 File Offset: 0x001CBBF0
		public override void Reset()
		{
			this.gameObject = null;
			this.text = null;
			this.inlineStyling = null;
			this.textureGradient = null;
			this.mainColor = null;
			this.gradientColor = null;
			this.useGradient = null;
			this.anchor = TextAnchor.LowerLeft;
			this.scale = null;
			this.kerning = null;
			this.maxChars = null;
			this.NumDrawnCharacters = null;
			this.commit = true;
		}

		// Token: 0x06005BAE RID: 23470 RVA: 0x001CDA5D File Offset: 0x001CBC5D
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetProperties();
			base.Finish();
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x001CDA74 File Offset: 0x001CBC74
		private void DoSetProperties()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			bool flag = false;
			if (this._textMesh.text != this.text.Value)
			{
				this._textMesh.text = this.text.Value;
				flag = true;
			}
			if (this._textMesh.inlineStyling != this.inlineStyling.Value)
			{
				this._textMesh.inlineStyling = this.inlineStyling.Value;
				flag = true;
			}
			if (this._textMesh.textureGradient != this.textureGradient.Value)
			{
				this._textMesh.textureGradient = this.textureGradient.Value;
				flag = true;
			}
			if (this._textMesh.useGradient != this.useGradient.Value)
			{
				this._textMesh.useGradient = this.useGradient.Value;
				flag = true;
			}
			if (this._textMesh.color != this.mainColor.Value)
			{
				this._textMesh.color = this.mainColor.Value;
				flag = true;
			}
			if (this._textMesh.color2 != this.gradientColor.Value)
			{
				this._textMesh.color2 = this.gradientColor.Value;
				flag = true;
			}
			this.scale.Value = this._textMesh.scale;
			this.kerning.Value = this._textMesh.kerning;
			this.maxChars.Value = this._textMesh.maxChars;
			this.NumDrawnCharacters.Value = this._textMesh.NumDrawnCharacters();
			this.textureGradient.Value = this._textMesh.textureGradient;
			if (this.commit.Value && flag)
			{
				this._textMesh.Commit();
			}
		}

		// Token: 0x04005713 RID: 22291
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005714 RID: 22292
		[Tooltip("The Text")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		// Token: 0x04005715 RID: 22293
		[Tooltip("InlineStyling")]
		[UIHint(UIHint.Variable)]
		public FsmBool inlineStyling;

		// Token: 0x04005716 RID: 22294
		[Tooltip("anchor")]
		public TextAnchor anchor;

		// Token: 0x04005717 RID: 22295
		[Tooltip("The anchor as a string (text Anchor setting will be ignore if set). \npossible values ( case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
		[UIHint(UIHint.FsmString)]
		public FsmString OrTextAnchorString;

		// Token: 0x04005718 RID: 22296
		[Tooltip("Kerning")]
		[UIHint(UIHint.Variable)]
		public FsmBool kerning;

		// Token: 0x04005719 RID: 22297
		[Tooltip("maxChars")]
		[UIHint(UIHint.Variable)]
		public FsmInt maxChars;

		// Token: 0x0400571A RID: 22298
		[Tooltip("number of drawn characters")]
		[UIHint(UIHint.Variable)]
		public FsmInt NumDrawnCharacters;

		// Token: 0x0400571B RID: 22299
		[Tooltip("The Main Color")]
		[UIHint(UIHint.Variable)]
		public FsmColor mainColor;

		// Token: 0x0400571C RID: 22300
		[Tooltip("The Gradient Color. Only used if gradient is true")]
		[UIHint(UIHint.Variable)]
		public FsmColor gradientColor;

		// Token: 0x0400571D RID: 22301
		[Tooltip("Use gradient")]
		[UIHint(UIHint.Variable)]
		public FsmBool useGradient;

		// Token: 0x0400571E RID: 22302
		[Tooltip("Texture gradient")]
		[UIHint(UIHint.Variable)]
		public FsmInt textureGradient;

		// Token: 0x0400571F RID: 22303
		[Tooltip("Scale")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;

		// Token: 0x04005720 RID: 22304
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x04005721 RID: 22305
		private tk2dTextMesh _textMesh;
	}
}
