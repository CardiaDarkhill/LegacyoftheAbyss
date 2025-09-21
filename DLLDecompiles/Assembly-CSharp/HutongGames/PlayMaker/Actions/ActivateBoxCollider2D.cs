using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9D RID: 2973
	[ActionCategory("Physics 2d")]
	public class ActivateBoxCollider2D : FsmStateAction
	{
		// Token: 0x06005BE9 RID: 23529 RVA: 0x001CE950 File Offset: 0x001CCB50
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06005BEA RID: 23530 RVA: 0x001CE968 File Offset: 0x001CCB68
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				BoxCollider2D boxCollider2D = ownerDefaultTarget ? ownerDefaultTarget.GetComponent<BoxCollider2D>() : null;
				if (boxCollider2D != null)
				{
					boxCollider2D.enabled = this.active.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x06005BEB RID: 23531 RVA: 0x001CE9C4 File Offset: 0x001CCBC4
		public override void OnExit()
		{
			if (this.resetOnExit && this.gameObject != null)
			{
				BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<BoxCollider2D>();
				if (component != null)
				{
					component.enabled = !this.active.Value;
				}
			}
		}

		// Token: 0x0400574B RID: 22347
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400574C RID: 22348
		public FsmBool active;

		// Token: 0x0400574D RID: 22349
		public bool resetOnExit;
	}
}
