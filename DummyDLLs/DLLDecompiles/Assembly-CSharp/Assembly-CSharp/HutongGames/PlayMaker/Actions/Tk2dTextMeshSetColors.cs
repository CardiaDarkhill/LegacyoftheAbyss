using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8D RID: 2957
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the colors of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetColors : FsmStateAction
	{
		// Token: 0x06005B90 RID: 23440 RVA: 0x001CD460 File Offset: 0x001CB660
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B91 RID: 23441 RVA: 0x001CD495 File Offset: 0x001CB695
		public override void Reset()
		{
			this.gameObject = null;
			this.mainColor = null;
			this.gradientColor = null;
			this.useGradient = false;
			this.commit = true;
			this.everyframe = false;
		}

		// Token: 0x06005B92 RID: 23442 RVA: 0x001CD4CB File Offset: 0x001CB6CB
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetColors();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B93 RID: 23443 RVA: 0x001CD4E7 File Offset: 0x001CB6E7
		public override void OnUpdate()
		{
			this.DoSetColors();
		}

		// Token: 0x06005B94 RID: 23444 RVA: 0x001CD4F0 File Offset: 0x001CB6F0
		private void DoSetColors()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			bool flag = false;
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
			if (this.commit.Value && flag)
			{
				this._textMesh.Commit();
			}
		}

		// Token: 0x040056FA RID: 22266
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056FB RID: 22267
		[Tooltip("Main color")]
		[UIHint(UIHint.FsmColor)]
		public FsmColor mainColor;

		// Token: 0x040056FC RID: 22268
		[Tooltip("Gradient color. Only used if gradient is true")]
		[UIHint(UIHint.FsmColor)]
		public FsmColor gradientColor;

		// Token: 0x040056FD RID: 22269
		[Tooltip("Use gradient.")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool useGradient;

		// Token: 0x040056FE RID: 22270
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x040056FF RID: 22271
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x04005700 RID: 22272
		private tk2dTextMesh _textMesh;
	}
}
