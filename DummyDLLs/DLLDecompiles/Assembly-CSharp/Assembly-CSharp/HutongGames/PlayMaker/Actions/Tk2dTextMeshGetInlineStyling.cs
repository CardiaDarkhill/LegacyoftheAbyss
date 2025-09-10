using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B82 RID: 2946
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the inline styling flag of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetInlineStyling : FsmStateAction
	{
		// Token: 0x06005B52 RID: 23378 RVA: 0x001CC998 File Offset: 0x001CAB98
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B53 RID: 23379 RVA: 0x001CC9CD File Offset: 0x001CABCD
		public override void Reset()
		{
			this.gameObject = null;
			this.inlineStyling = null;
			this.everyframe = false;
		}

		// Token: 0x06005B54 RID: 23380 RVA: 0x001CC9E4 File Offset: 0x001CABE4
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetInlineStyling();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B55 RID: 23381 RVA: 0x001CCA00 File Offset: 0x001CAC00
		public override void OnUpdate()
		{
			this.DoGetInlineStyling();
		}

		// Token: 0x06005B56 RID: 23382 RVA: 0x001CCA08 File Offset: 0x001CAC08
		private void DoGetInlineStyling()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			this.inlineStyling.Value = this._textMesh.inlineStyling;
		}

		// Token: 0x040056C7 RID: 22215
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056C8 RID: 22216
		[RequiredField]
		[Tooltip("The max number of characters")]
		[UIHint(UIHint.Variable)]
		public FsmBool inlineStyling;

		// Token: 0x040056C9 RID: 22217
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056CA RID: 22218
		private tk2dTextMesh _textMesh;
	}
}
