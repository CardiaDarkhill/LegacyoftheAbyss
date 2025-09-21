using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D44 RID: 3396
	[ActionCategory("Physics 2d")]
	public class SetPolygonColliderTrigger : FsmStateAction
	{
		// Token: 0x060063A5 RID: 25509 RVA: 0x001F6CF6 File Offset: 0x001F4EF6
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = false;
		}

		// Token: 0x060063A6 RID: 25510 RVA: 0x001F6D0C File Offset: 0x001F4F0C
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				PolygonCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<PolygonCollider2D>();
				if (component != null)
				{
					component.isTrigger = this.trigger.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x040061F9 RID: 25081
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061FA RID: 25082
		public FsmBool trigger;
	}
}
