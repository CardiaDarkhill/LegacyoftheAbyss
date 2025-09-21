using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C0 RID: 4800
	public class GetHealthManagerPhysicalPusher : FsmStateAction
	{
		// Token: 0x06007D8A RID: 32138 RVA: 0x00256A7D File Offset: 0x00254C7D
		public override void Reset()
		{
			this.target = null;
			this.storeGameObject = null;
		}

		// Token: 0x06007D8B RID: 32139 RVA: 0x00256A90 File Offset: 0x00254C90
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component)
				{
					this.storeGameObject.Value = component.GetPhysicalPusher();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D7A RID: 32122
		public FsmOwnerDefault target;

		// Token: 0x04007D7B RID: 32123
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;
	}
}
