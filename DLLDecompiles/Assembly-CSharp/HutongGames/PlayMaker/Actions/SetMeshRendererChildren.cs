using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D33 RID: 3379
	[ActionCategory("GameObject")]
	[Tooltip("Set Mesh Renderer of object's children to active or inactive. Can only be one Mesh Renderer on each object. ")]
	public class SetMeshRendererChildren : FsmStateAction
	{
		// Token: 0x06006365 RID: 25445 RVA: 0x001F616F File Offset: 0x001F436F
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06006366 RID: 25446 RVA: 0x001F6184 File Offset: 0x001F4384
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (object obj in ownerDefaultTarget.transform)
					{
						MeshRenderer component = ((Transform)obj).GetComponent<MeshRenderer>();
						if (component != null)
						{
							component.enabled = this.active.Value;
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040061C1 RID: 25025
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061C2 RID: 25026
		public FsmBool active;
	}
}
