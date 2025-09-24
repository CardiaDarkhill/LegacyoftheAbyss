using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8F RID: 2959
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the inlineStyling flag of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetInlineStyling : FsmStateAction
	{
		// Token: 0x06005B9B RID: 23451 RVA: 0x001CD6F4 File Offset: 0x001CB8F4
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B9C RID: 23452 RVA: 0x001CD729 File Offset: 0x001CB929
		public override void Reset()
		{
			this.gameObject = null;
			this.inlineStyling = true;
			this.commit = true;
		}

		// Token: 0x06005B9D RID: 23453 RVA: 0x001CD74A File Offset: 0x001CB94A
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetInlineStyling();
			base.Finish();
		}

		// Token: 0x06005B9E RID: 23454 RVA: 0x001CD760 File Offset: 0x001CB960
		private void DoSetInlineStyling()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			if (this._textMesh.inlineStyling != this.inlineStyling.Value)
			{
				this._textMesh.inlineStyling = this.inlineStyling.Value;
				if (this.commit.Value)
				{
					this._textMesh.Commit();
				}
			}
		}

		// Token: 0x04005705 RID: 22277
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005706 RID: 22278
		[Tooltip("Does the text features inline styling?")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool inlineStyling;

		// Token: 0x04005707 RID: 22279
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x04005708 RID: 22280
		private tk2dTextMesh _textMesh;
	}
}
