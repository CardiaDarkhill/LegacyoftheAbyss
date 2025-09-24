using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B93 RID: 2963
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the scale of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetScale : FsmStateAction
	{
		// Token: 0x06005BB1 RID: 23473 RVA: 0x001CDC74 File Offset: 0x001CBE74
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005BB2 RID: 23474 RVA: 0x001CDCA9 File Offset: 0x001CBEA9
		public override void Reset()
		{
			this.gameObject = null;
			this.scale = null;
			this.commit = true;
			this.everyframe = false;
		}

		// Token: 0x06005BB3 RID: 23475 RVA: 0x001CDCCC File Offset: 0x001CBECC
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetScale();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005BB4 RID: 23476 RVA: 0x001CDCE8 File Offset: 0x001CBEE8
		public override void OnUpdate()
		{
			this.DoSetScale();
		}

		// Token: 0x06005BB5 RID: 23477 RVA: 0x001CDCF0 File Offset: 0x001CBEF0
		private void DoSetScale()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			if (this._textMesh.scale != this.scale.Value)
			{
				this._textMesh.scale = this.scale.Value;
				if (this.commit.Value)
				{
					this._textMesh.Commit();
				}
			}
		}

		// Token: 0x04005722 RID: 22306
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005723 RID: 22307
		[Tooltip("The scale")]
		[UIHint(UIHint.FsmVector3)]
		public FsmVector3 scale;

		// Token: 0x04005724 RID: 22308
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool commit;

		// Token: 0x04005725 RID: 22309
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x04005726 RID: 22310
		private tk2dTextMesh _textMesh;
	}
}
