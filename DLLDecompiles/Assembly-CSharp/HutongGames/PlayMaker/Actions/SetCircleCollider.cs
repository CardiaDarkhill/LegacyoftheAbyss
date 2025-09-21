using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013B0 RID: 5040
	[ActionCategory("Physics 2d")]
	[Tooltip("Set BoxCollider2D to active or inactive. Can only be one collider on object. ")]
	public class SetCircleCollider : FsmStateAction
	{
		// Token: 0x0600811F RID: 33055 RVA: 0x0026062C File Offset: 0x0025E82C
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06008120 RID: 33056 RVA: 0x00260644 File Offset: 0x0025E844
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				CircleCollider2D component = ownerDefaultTarget.GetComponent<CircleCollider2D>();
				if (component != null)
				{
					component.enabled = this.active.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04008059 RID: 32857
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400805A RID: 32858
		public FsmBool active;
	}
}
