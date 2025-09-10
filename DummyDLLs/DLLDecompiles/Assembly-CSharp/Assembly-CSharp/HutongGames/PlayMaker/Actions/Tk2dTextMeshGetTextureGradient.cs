using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8A RID: 2954
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the texture gradient of the font currently applied to a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetTextureGradient : FsmStateAction
	{
		// Token: 0x06005B7E RID: 23422 RVA: 0x001CD044 File Offset: 0x001CB244
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B7F RID: 23423 RVA: 0x001CD079 File Offset: 0x001CB279
		public override void Reset()
		{
			this.gameObject = null;
			this.textureGradient = 0;
			this.everyframe = false;
		}

		// Token: 0x06005B80 RID: 23424 RVA: 0x001CD095 File Offset: 0x001CB295
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoGetTextureGradient();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005B81 RID: 23425 RVA: 0x001CD0B1 File Offset: 0x001CB2B1
		public override void OnUpdate()
		{
			this.DoGetTextureGradient();
		}

		// Token: 0x06005B82 RID: 23426 RVA: 0x001CD0BC File Offset: 0x001CB2BC
		private void DoGetTextureGradient()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			this.textureGradient.Value = this._textMesh.textureGradient;
		}

		// Token: 0x040056EF RID: 22255
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056F0 RID: 22256
		[Tooltip("The Gradient Id")]
		[UIHint(UIHint.Variable)]
		public FsmInt textureGradient;

		// Token: 0x040056F1 RID: 22257
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x040056F2 RID: 22258
		private tk2dTextMesh _textMesh;
	}
}
