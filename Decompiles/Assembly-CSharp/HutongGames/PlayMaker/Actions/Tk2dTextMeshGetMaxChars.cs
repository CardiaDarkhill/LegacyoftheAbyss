using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B84 RID: 2948
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the maximum characters number of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetMaxChars : FsmStateAction
	{
		// Token: 0x06005B5E RID: 23390 RVA: 0x001CCB10 File Offset: 0x001CAD10
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B5F RID: 23391 RVA: 0x001CCB45 File Offset: 0x001CAD45
		public override void Reset()
		{
			this.gameObject = null;
			this.maxChars = null;
			this.everyframe = false;
		}

		// Token: 0x06005B60 RID: 23392 RVA: 0x001CCB5C File Offset: 0x001CAD5C
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetMaxChars();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B61 RID: 23393 RVA: 0x001CCB78 File Offset: 0x001CAD78
		public override void OnUpdate()
		{
			this.DoGetMaxChars();
		}

		// Token: 0x06005B62 RID: 23394 RVA: 0x001CCB80 File Offset: 0x001CAD80
		private void DoGetMaxChars()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			this.maxChars.Value = this._textMesh.maxChars;
		}

		// Token: 0x040056CF RID: 22223
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056D0 RID: 22224
		[Tooltip("The max number of characters")]
		[UIHint(UIHint.Variable)]
		public FsmInt maxChars;

		// Token: 0x040056D1 RID: 22225
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056D2 RID: 22226
		private tk2dTextMesh _textMesh;
	}
}
