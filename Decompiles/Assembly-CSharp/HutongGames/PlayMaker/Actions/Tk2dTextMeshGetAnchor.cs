using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7F RID: 2943
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the anchor of a TextMesh. \nThe anchor is stored as a string. tk2dTextMeshSetAnchor can work with this string. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetAnchor : FsmStateAction
	{
		// Token: 0x06005B41 RID: 23361 RVA: 0x001CC70C File Offset: 0x001CA90C
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B42 RID: 23362 RVA: 0x001CC741 File Offset: 0x001CA941
		public override void Reset()
		{
			this.gameObject = null;
			this.textAnchorAsString = "";
			this.everyframe = false;
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x001CC761 File Offset: 0x001CA961
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetAnchor();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x001CC77D File Offset: 0x001CA97D
		public override void OnUpdate()
		{
			this.DoGetAnchor();
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001CC788 File Offset: 0x001CA988
		private void DoGetAnchor()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component");
				return;
			}
			this.textAnchorAsString.Value = this._textMesh.anchor.ToString();
		}

		// Token: 0x040056BA RID: 22202
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056BB RID: 22203
		[RequiredField]
		[Tooltip("The anchor as a string. \npossible values: LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
		[UIHint(UIHint.Variable)]
		public FsmString textAnchorAsString;

		// Token: 0x040056BC RID: 22204
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056BD RID: 22205
		private tk2dTextMesh _textMesh;
	}
}
