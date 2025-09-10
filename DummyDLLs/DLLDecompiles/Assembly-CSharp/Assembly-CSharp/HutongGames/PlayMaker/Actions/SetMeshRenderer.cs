using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D32 RID: 3378
	[ActionCategory("GameObject")]
	[Tooltip("Set Mesh Renderer to active or inactive. Can only be one Mesh Renderer on object. ")]
	public class SetMeshRenderer : FsmStateAction
	{
		// Token: 0x06006362 RID: 25442 RVA: 0x001F60FA File Offset: 0x001F42FA
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x001F6110 File Offset: 0x001F4310
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					MeshRenderer component = ownerDefaultTarget.GetComponent<MeshRenderer>();
					if (component != null)
					{
						component.enabled = this.active.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040061BF RID: 25023
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061C0 RID: 25024
		public FsmBool active;
	}
}
