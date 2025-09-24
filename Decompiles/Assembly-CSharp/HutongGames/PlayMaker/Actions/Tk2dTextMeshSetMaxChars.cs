using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B90 RID: 2960
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the maximum characters number of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetMaxChars : FsmStateAction
	{
		// Token: 0x06005BA0 RID: 23456 RVA: 0x001CD7D8 File Offset: 0x001CB9D8
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005BA1 RID: 23457 RVA: 0x001CD80D File Offset: 0x001CBA0D
		public override void Reset()
		{
			this.gameObject = null;
			this.maxChars = 30;
			this.commit = true;
			this.everyframe = false;
		}

		// Token: 0x06005BA2 RID: 23458 RVA: 0x001CD836 File Offset: 0x001CBA36
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetMaxChars();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005BA3 RID: 23459 RVA: 0x001CD852 File Offset: 0x001CBA52
		public override void OnUpdate()
		{
			this.DoSetMaxChars();
		}

		// Token: 0x06005BA4 RID: 23460 RVA: 0x001CD85C File Offset: 0x001CBA5C
		private void DoSetMaxChars()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: ");
				return;
			}
			if (this._textMesh.maxChars != this.maxChars.Value)
			{
				this._textMesh.maxChars = this.maxChars.Value;
				if (this.commit.Value)
				{
					this._textMesh.Commit();
				}
			}
		}

		// Token: 0x04005709 RID: 22281
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400570A RID: 22282
		[Tooltip("The max number of characters")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt maxChars;

		// Token: 0x0400570B RID: 22283
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x0400570C RID: 22284
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x0400570D RID: 22285
		private tk2dTextMesh _textMesh;
	}
}
