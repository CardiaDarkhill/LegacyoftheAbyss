using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B83 RID: 2947
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Check that inline styling can indeed be used ( the font needs to have texture gradients for inline styling to work). \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetInlineStylingIsAvailable : FsmStateAction
	{
		// Token: 0x06005B58 RID: 23384 RVA: 0x001CCA44 File Offset: 0x001CAC44
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B59 RID: 23385 RVA: 0x001CCA79 File Offset: 0x001CAC79
		public override void Reset()
		{
			this.gameObject = null;
			this.InlineStylingAvailable = null;
			this.everyframe = false;
		}

		// Token: 0x06005B5A RID: 23386 RVA: 0x001CCA90 File Offset: 0x001CAC90
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetInlineStylingAvailable();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B5B RID: 23387 RVA: 0x001CCAAC File Offset: 0x001CACAC
		public override void OnUpdate()
		{
			this.DoGetInlineStylingAvailable();
		}

		// Token: 0x06005B5C RID: 23388 RVA: 0x001CCAB4 File Offset: 0x001CACB4
		private void DoGetInlineStylingAvailable()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			this.InlineStylingAvailable.Value = (this._textMesh.inlineStyling && this._textMesh.font.textureGradients);
		}

		// Token: 0x040056CB RID: 22219
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056CC RID: 22220
		[RequiredField]
		[Tooltip("Is inline styling available? true if inlineStyling is true AND the font texturGradients is true")]
		[UIHint(UIHint.Variable)]
		public FsmBool InlineStylingAvailable;

		// Token: 0x040056CD RID: 22221
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056CE RID: 22222
		private tk2dTextMesh _textMesh;
	}
}
