using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B91 RID: 2961
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the pixelPerfect flag of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetPixelPerfect : FsmStateAction
	{
		// Token: 0x06005BA6 RID: 23462 RVA: 0x001CD8D4 File Offset: 0x001CBAD4
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005BA7 RID: 23463 RVA: 0x001CD909 File Offset: 0x001CBB09
		public override void Reset()
		{
			this.gameObject = null;
			this.pixelPerfect = true;
			this.commit = true;
			this.everyframe = false;
		}

		// Token: 0x06005BA8 RID: 23464 RVA: 0x001CD931 File Offset: 0x001CBB31
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetPixelPerfect();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005BA9 RID: 23465 RVA: 0x001CD94D File Offset: 0x001CBB4D
		public override void OnUpdate()
		{
			this.DoSetPixelPerfect();
		}

		// Token: 0x06005BAA RID: 23466 RVA: 0x001CD958 File Offset: 0x001CBB58
		private void DoSetPixelPerfect()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			if (this.pixelPerfect.Value)
			{
				this._textMesh.MakePixelPerfect();
				if (this.commit.Value)
				{
					this._textMesh.Commit();
				}
			}
		}

		// Token: 0x0400570E RID: 22286
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400570F RID: 22287
		[Tooltip("Does the text needs to be pixelPerfect")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool pixelPerfect;

		// Token: 0x04005710 RID: 22288
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x04005711 RID: 22289
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x04005712 RID: 22290
		private tk2dTextMesh _textMesh;
	}
}
