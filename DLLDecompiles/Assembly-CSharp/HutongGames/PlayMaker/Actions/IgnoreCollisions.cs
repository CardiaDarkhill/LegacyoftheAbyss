using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8A RID: 3210
	public class IgnoreCollisions : FsmStateAction
	{
		// Token: 0x0600608F RID: 24719 RVA: 0x001EA034 File Offset: 0x001E8234
		public override void Reset()
		{
			this.Target = null;
			this.Other = null;
		}

		// Token: 0x06006090 RID: 24720 RVA: 0x001EA044 File Offset: 0x001E8244
		public override void OnEnter()
		{
			Collider2D[] componentsInChildren = this.Target.GetSafe(this).GetComponentsInChildren<Collider2D>(true);
			Collider2D[] componentsInChildren2 = this.Other.Value.GetComponentsInChildren<Collider2D>(true);
			foreach (Collider2D collider in componentsInChildren)
			{
				foreach (Collider2D collider2 in componentsInChildren2)
				{
					Physics2D.IgnoreCollision(collider, collider2);
				}
			}
			base.Finish();
		}

		// Token: 0x04005E12 RID: 24082
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04005E13 RID: 24083
		[RequiredField]
		public FsmGameObject Other;
	}
}
