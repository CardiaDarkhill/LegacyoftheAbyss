using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA6 RID: 3750
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroys the Owner of the Fsm! Useful for spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
	public class DestroySelf : FsmStateAction
	{
		// Token: 0x06006A46 RID: 27206 RVA: 0x00213737 File Offset: 0x00211937
		public override void Reset()
		{
			this.detachChildren = false;
		}

		// Token: 0x06006A47 RID: 27207 RVA: 0x00213745 File Offset: 0x00211945
		public override void OnEnter()
		{
			if (base.Owner != null)
			{
				if (this.detachChildren.Value)
				{
					base.Owner.transform.DetachChildren();
				}
				Object.Destroy(base.Owner);
			}
			base.Finish();
		}

		// Token: 0x040069A2 RID: 27042
		[Tooltip("Detach children before destroying the Owner.")]
		public FsmBool detachChildren;
	}
}
