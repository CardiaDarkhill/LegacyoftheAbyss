using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B95 RID: 2965
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the texture gradient of the font currently applied to a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetTextureGradient : FsmStateAction
	{
		// Token: 0x06005BBD RID: 23485 RVA: 0x001CDE98 File Offset: 0x001CC098
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x001CDECD File Offset: 0x001CC0CD
		public override void Reset()
		{
			this.gameObject = null;
			this.textureGradient = 0;
			this.commit = true;
			this.everyframe = false;
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x001CDEF5 File Offset: 0x001CC0F5
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoSetTextureGradient();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005BC0 RID: 23488 RVA: 0x001CDF11 File Offset: 0x001CC111
		public override void OnUpdate()
		{
			this.DoSetTextureGradient();
		}

		// Token: 0x06005BC1 RID: 23489 RVA: 0x001CDF1C File Offset: 0x001CC11C
		private void DoSetTextureGradient()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			if (this._textMesh.textureGradient != this.textureGradient.Value)
			{
				this._textMesh.textureGradient = this.textureGradient.Value;
				if (this.commit.Value)
				{
					this._textMesh.Commit();
				}
			}
		}

		// Token: 0x0400572C RID: 22316
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400572D RID: 22317
		[Tooltip("The Gradient Id")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt textureGradient;

		// Token: 0x0400572E RID: 22318
		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
		public FsmBool commit;

		// Token: 0x0400572F RID: 22319
		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		// Token: 0x04005730 RID: 22320
		private tk2dTextMesh _textMesh;
	}
}
