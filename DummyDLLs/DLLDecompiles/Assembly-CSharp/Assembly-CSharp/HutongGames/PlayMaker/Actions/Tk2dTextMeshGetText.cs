using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B89 RID: 2953
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the text of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetText : FsmStateAction
	{
		// Token: 0x06005B78 RID: 23416 RVA: 0x001CCF78 File Offset: 0x001CB178
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B79 RID: 23417 RVA: 0x001CCFAD File Offset: 0x001CB1AD
		public override void Reset()
		{
			this.gameObject = null;
			this.text = null;
			this.everyframe = false;
		}

		// Token: 0x06005B7A RID: 23418 RVA: 0x001CCFC4 File Offset: 0x001CB1C4
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetText();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B7B RID: 23419 RVA: 0x001CCFE0 File Offset: 0x001CB1E0
		public override void OnUpdate()
		{
			this.DoGetText();
		}

		// Token: 0x06005B7C RID: 23420 RVA: 0x001CCFE8 File Offset: 0x001CB1E8
		private void DoGetText()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			this.text.Value = this._textMesh.text;
		}

		// Token: 0x040056EB RID: 22251
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056EC RID: 22252
		[Tooltip("The text")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		// Token: 0x040056ED RID: 22253
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056EE RID: 22254
		private tk2dTextMesh _textMesh;
	}
}
