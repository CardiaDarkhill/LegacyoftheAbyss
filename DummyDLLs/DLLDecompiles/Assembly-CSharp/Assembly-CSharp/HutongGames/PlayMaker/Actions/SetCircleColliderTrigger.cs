using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D25 RID: 3365
	[ActionCategory("Physics 2d")]
	public class SetCircleColliderTrigger : FsmStateAction
	{
		// Token: 0x0600632F RID: 25391 RVA: 0x001F58E0 File Offset: 0x001F3AE0
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = false;
		}

		// Token: 0x06006330 RID: 25392 RVA: 0x001F58F8 File Offset: 0x001F3AF8
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				CircleCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<CircleCollider2D>();
				if (component != null)
				{
					component.isTrigger = this.trigger.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x0400619B RID: 24987
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400619C RID: 24988
		public FsmBool trigger;
	}
}
