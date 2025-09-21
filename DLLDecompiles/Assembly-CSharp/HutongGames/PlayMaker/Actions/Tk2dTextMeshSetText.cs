using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B94 RID: 2964
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the text of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W719")]
	public class Tk2dTextMeshSetText : FsmStateAction
	{
		// Token: 0x06005BB7 RID: 23479 RVA: 0x001CDD80 File Offset: 0x001CBF80
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005BB8 RID: 23480 RVA: 0x001CDDB5 File Offset: 0x001CBFB5
		public override void Reset()
		{
			this.gameObject = null;
			this.text = "";
			this.commit = true;
			this.everyframe = false;
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x001CDDE1 File Offset: 0x001CBFE1
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetText();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005BBA RID: 23482 RVA: 0x001CDDFD File Offset: 0x001CBFFD
		public override void OnUpdate()
		{
			this.DoSetText();
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x001CDE08 File Offset: 0x001CC008
		private void DoSetText()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			if (this._textMesh.text != this.text.Value)
			{
				this._textMesh.text = this.text.Value;
				if (this.commit.Value)
				{
					this._textMesh.Commit();
				}
			}
		}

		// Token: 0x04005727 RID: 22311
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005728 RID: 22312
		[Tooltip("The text")]
		[UIHint(UIHint.FsmString)]
		public FsmString text;

		// Token: 0x04005729 RID: 22313
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x0400572A RID: 22314
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x0400572B RID: 22315
		private tk2dTextMesh _textMesh;
	}
}
