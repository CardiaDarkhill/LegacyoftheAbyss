using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D52 RID: 3410
	[ActionCategory("GameObject")]
	[Tooltip("Set sprite renderer to active or inactive. Can only be one sprite renderer on object. ")]
	public class SetSpriteRenderer : FsmStateAction
	{
		// Token: 0x060063E7 RID: 25575 RVA: 0x001F7E08 File Offset: 0x001F6008
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x060063E8 RID: 25576 RVA: 0x001F7E20 File Offset: 0x001F6020
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.enabled = this.active.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400624B RID: 25163
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400624C RID: 25164
		public FsmBool active;
	}
}
