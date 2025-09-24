using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B81 RID: 2945
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the font of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetFont : FsmStateAction
	{
		// Token: 0x06005B4D RID: 23373 RVA: 0x001CC8D4 File Offset: 0x001CAAD4
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x001CC909 File Offset: 0x001CAB09
		public override void Reset()
		{
			this.gameObject = null;
			this.font = null;
		}

		// Token: 0x06005B4F RID: 23375 RVA: 0x001CC919 File Offset: 0x001CAB19
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetFont();
			base.Finish();
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x001CC930 File Offset: 0x001CAB30
		private void DoGetFont()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			GameObject value = this.font.Value;
			if (value == null)
			{
				return;
			}
			value.GetComponent<tk2dFont>() == null;
		}

		// Token: 0x040056C4 RID: 22212
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056C5 RID: 22213
		[RequiredField]
		[Tooltip("The font gameObject")]
		[UIHint(UIHint.FsmGameObject)]
		public FsmGameObject font;

		// Token: 0x040056C6 RID: 22214
		private tk2dTextMesh _textMesh;
	}
}
