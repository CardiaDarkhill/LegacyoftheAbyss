using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7E RID: 2942
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Commit a TextMesh. This is so you can change multiple parameters without reconstructing the mesh repeatedly, simply use that after you have set all the different properties. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W723")]
	public class Tk2dTextMeshCommit : FsmStateAction
	{
		// Token: 0x06005B3C RID: 23356 RVA: 0x001CC674 File Offset: 0x001CA874
		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
		}

		// Token: 0x06005B3D RID: 23357 RVA: 0x001CC6A9 File Offset: 0x001CA8A9
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06005B3E RID: 23358 RVA: 0x001CC6B2 File Offset: 0x001CA8B2
		public override void OnEnter()
		{
			this._getTextMesh();
			this.DoCommit();
			base.Finish();
		}

		// Token: 0x06005B3F RID: 23359 RVA: 0x001CC6C6 File Offset: 0x001CA8C6
		private void DoCommit()
		{
			if (this._textMesh == null)
			{
				base.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
				return;
			}
			this._textMesh.Commit();
		}

		// Token: 0x040056B8 RID: 22200
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056B9 RID: 22201
		private tk2dTextMesh _textMesh;
	}
}
