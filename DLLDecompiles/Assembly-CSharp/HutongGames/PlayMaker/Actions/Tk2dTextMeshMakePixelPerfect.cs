using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8B RID: 2955
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Make a TextMesh pixelPerfect. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshMakePixelPerfect : FsmStateAction
	{
		// Token: 0x06005B84 RID: 23428 RVA: 0x001CD118 File Offset: 0x001CB318
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x001CD14D File Offset: 0x001CB34D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06005B86 RID: 23430 RVA: 0x001CD156 File Offset: 0x001CB356
		public override void OnEnter()
		{
			this._getTextMesh();
			this.MakePixelPerfect();
			base.Finish();
		}

		// Token: 0x06005B87 RID: 23431 RVA: 0x001CD16A File Offset: 0x001CB36A
		private void MakePixelPerfect()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component ");
				return;
			}
			this._textMesh.MakePixelPerfect();
		}

		// Token: 0x040056F3 RID: 22259
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056F4 RID: 22260
		private tk2dTextMesh _textMesh;
	}
}
