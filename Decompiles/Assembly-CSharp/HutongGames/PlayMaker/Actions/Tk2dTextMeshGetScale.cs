using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B88 RID: 2952
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the scale of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetScale : FsmStateAction
	{
		// Token: 0x06005B72 RID: 23410 RVA: 0x001CCE92 File Offset: 0x001CB092
		private void _getTextMesh()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				return;
			}
			this._textMesh = this.go.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B73 RID: 23411 RVA: 0x001CCECB File Offset: 0x001CB0CB
		public override void Reset()
		{
			this.gameObject = null;
			this.scale = null;
			this.everyframe = false;
		}

		// Token: 0x06005B74 RID: 23412 RVA: 0x001CCEE2 File Offset: 0x001CB0E2
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetScale();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B75 RID: 23413 RVA: 0x001CCEFE File Offset: 0x001CB0FE
		public override void OnUpdate()
		{
			this.DoGetScale();
		}

		// Token: 0x06005B76 RID: 23414 RVA: 0x001CCF08 File Offset: 0x001CB108
		private void DoGetScale()
		{
			if (this.go == null)
			{
				return;
			}
			if (this._textMesh == null)
			{
				Debug.Log(this._textMesh);
				base.LogError("Missing tk2dTextMesh component: " + this.go.name);
				return;
			}
			this.scale.Value = this._textMesh.scale;
		}

		// Token: 0x040056E6 RID: 22246
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056E7 RID: 22247
		[RequiredField]
		[Tooltip("The scale")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;

		// Token: 0x040056E8 RID: 22248
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056E9 RID: 22249
		private GameObject go;

		// Token: 0x040056EA RID: 22250
		private tk2dTextMesh _textMesh;
	}
}
