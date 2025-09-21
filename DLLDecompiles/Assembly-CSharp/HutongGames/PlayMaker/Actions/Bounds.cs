using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD4 RID: 3028
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("gets the local or gloabal bounding box measures")]
	public class Bounds : FsmStateAction
	{
		// Token: 0x06005CE0 RID: 23776 RVA: 0x001D317C File Offset: 0x001D137C
		public override void Reset()
		{
			this.gameObject1 = null;
			this.everyFrame = false;
			this.global = false;
		}

		// Token: 0x06005CE1 RID: 23777 RVA: 0x001D3194 File Offset: 0x001D1394
		public void GetEm()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject1);
			if (this.global)
			{
				this.scale.Value = ownerDefaultTarget.GetComponent<Renderer>().bounds.size;
				return;
			}
			Mesh sharedMesh = ownerDefaultTarget.GetComponent<MeshFilter>().sharedMesh;
			this.scale.Value = sharedMesh.bounds.size;
		}

		// Token: 0x06005CE2 RID: 23778 RVA: 0x001D31FF File Offset: 0x001D13FF
		public override void OnEnter()
		{
			this.GetEm();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CE3 RID: 23779 RVA: 0x001D3215 File Offset: 0x001D1415
		public override void OnUpdate()
		{
			this.GetEm();
		}

		// Token: 0x0400587A RID: 22650
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		// Token: 0x0400587B RID: 22651
		[Tooltip("gets the local or global bounding box scale")]
		public FsmVector3 scale;

		// Token: 0x0400587C RID: 22652
		[Tooltip("Should the scale be global? If it's rotated you probably want local axis for the scale")]
		public bool global;

		// Token: 0x0400587D RID: 22653
		public bool everyFrame;
	}
}
