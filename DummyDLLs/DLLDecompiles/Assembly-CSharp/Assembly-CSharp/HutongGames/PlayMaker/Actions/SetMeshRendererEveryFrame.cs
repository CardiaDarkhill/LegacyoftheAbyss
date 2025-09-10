using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D34 RID: 3380
	[ActionCategory("GameObject")]
	[Tooltip("Set Mesh Renderer to active or inactive. Can only be one Mesh Renderer on object. ")]
	public class SetMeshRendererEveryFrame : FsmStateAction
	{
		// Token: 0x06006368 RID: 25448 RVA: 0x001F6228 File Offset: 0x001F4428
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06006369 RID: 25449 RVA: 0x001F6240 File Offset: 0x001F4440
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.mr = ownerDefaultTarget.GetComponent<MeshRenderer>();
				}
			}
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x001F627C File Offset: 0x001F447C
		public override void OnUpdate()
		{
			if (this.mr != null)
			{
				this.mr.enabled = this.active.Value;
			}
		}

		// Token: 0x040061C3 RID: 25027
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061C4 RID: 25028
		public FsmBool active;

		// Token: 0x040061C5 RID: 25029
		private MeshRenderer mr;
	}
}
