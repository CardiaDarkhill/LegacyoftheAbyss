using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D24 RID: 3364
	[ActionCategory("Physics 2d")]
	[Tooltip("Set BoxCollider2D to active or inactive. Can only be one collider on object. ")]
	public class SetBoxColliderTrigger : FsmStateAction
	{
		// Token: 0x0600632C RID: 25388 RVA: 0x001F5876 File Offset: 0x001F3A76
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = false;
		}

		// Token: 0x0600632D RID: 25389 RVA: 0x001F588C File Offset: 0x001F3A8C
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<BoxCollider2D>();
				if (component != null)
				{
					component.isTrigger = this.trigger.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04006199 RID: 24985
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400619A RID: 24986
		public FsmBool trigger;
	}
}
