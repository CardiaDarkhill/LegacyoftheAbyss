using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013B1 RID: 5041
	[ActionCategory("Physics 2d")]
	[Tooltip("Set BoxCollider2D to active or inactive. Can only be one collider on object. ")]
	public class SetCircleColliderRadius : FsmStateAction
	{
		// Token: 0x06008122 RID: 33058 RVA: 0x002606A4 File Offset: 0x0025E8A4
		public override void Reset()
		{
			this.gameObject = null;
			this.radius = null;
		}

		// Token: 0x06008123 RID: 33059 RVA: 0x002606B4 File Offset: 0x0025E8B4
		public override void OnEnter()
		{
			this.SetRadius();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06008124 RID: 33060 RVA: 0x002606CA File Offset: 0x0025E8CA
		public override void OnUpdate()
		{
			this.SetRadius();
		}

		// Token: 0x06008125 RID: 33061 RVA: 0x002606D4 File Offset: 0x0025E8D4
		private void SetRadius()
		{
			if (this.gameObject != null)
			{
				CircleCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<CircleCollider2D>();
				if (component != null)
				{
					component.radius = this.radius.Value;
				}
			}
		}

		// Token: 0x0400805B RID: 32859
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400805C RID: 32860
		public FsmFloat radius;

		// Token: 0x0400805D RID: 32861
		public bool everyFrame;
	}
}
