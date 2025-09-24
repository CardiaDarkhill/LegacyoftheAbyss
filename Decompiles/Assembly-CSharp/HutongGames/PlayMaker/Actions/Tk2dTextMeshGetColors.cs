using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B80 RID: 2944
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the colors of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetColors : FsmStateAction
	{
		// Token: 0x06005B47 RID: 23367 RVA: 0x001CC7DC File Offset: 0x001CA9DC
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x001CC811 File Offset: 0x001CAA11
		public override void Reset()
		{
			this.gameObject = null;
			this.mainColor = null;
			this.gradientColor = null;
			this.useGradient = false;
			this.everyframe = false;
		}

		// Token: 0x06005B49 RID: 23369 RVA: 0x001CC83B File Offset: 0x001CAA3B
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetColors();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x001CC857 File Offset: 0x001CAA57
		public override void OnUpdate()
		{
			this.DoGetColors();
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001CC860 File Offset: 0x001CAA60
		private void DoGetColors()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			this.useGradient.Value = this._textMesh.useGradient;
			this.mainColor.Value = this._textMesh.color;
			this.gradientColor.Value = this._textMesh.color2;
		}

		// Token: 0x040056BE RID: 22206
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056BF RID: 22207
		[Tooltip("Main color")]
		[UIHint(UIHint.Variable)]
		public FsmColor mainColor;

		// Token: 0x040056C0 RID: 22208
		[Tooltip("Gradient color. Only used if gradient is true")]
		[UIHint(UIHint.Variable)]
		public FsmColor gradientColor;

		// Token: 0x040056C1 RID: 22209
		[Tooltip("Use gradient.")]
		[UIHint(UIHint.Variable)]
		public FsmBool useGradient;

		// Token: 0x040056C2 RID: 22210
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056C3 RID: 22211
		private tk2dTextMesh _textMesh;
	}
}
