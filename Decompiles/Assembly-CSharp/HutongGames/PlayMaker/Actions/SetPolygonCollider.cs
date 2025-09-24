using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D43 RID: 3395
	[ActionCategory("Physics 2d")]
	[Tooltip("Set PolygonCollider to active or inactive. Can only be one collider on object. ")]
	public class SetPolygonCollider : FsmStateAction
	{
		// Token: 0x060063A1 RID: 25505 RVA: 0x001F6C1E File Offset: 0x001F4E1E
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x060063A2 RID: 25506 RVA: 0x001F6C34 File Offset: 0x001F4E34
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				PolygonCollider2D polygonCollider2D = ownerDefaultTarget ? ownerDefaultTarget.GetComponent<PolygonCollider2D>() : null;
				if (polygonCollider2D != null)
				{
					polygonCollider2D.enabled = this.active.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x001F6C90 File Offset: 0x001F4E90
		public override void OnExit()
		{
			if (this.resetOnExit && this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				PolygonCollider2D polygonCollider2D = ownerDefaultTarget ? ownerDefaultTarget.GetComponent<PolygonCollider2D>() : null;
				if (polygonCollider2D != null)
				{
					polygonCollider2D.enabled = !this.active.Value;
				}
			}
		}

		// Token: 0x040061F6 RID: 25078
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061F7 RID: 25079
		public FsmBool active;

		// Token: 0x040061F8 RID: 25080
		public bool resetOnExit;
	}
}
