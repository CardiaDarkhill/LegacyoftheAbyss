using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001303 RID: 4867
	public class SetProjectileVelocityManager : FsmStateAction
	{
		// Token: 0x06007E86 RID: 32390 RVA: 0x002593D4 File Offset: 0x002575D4
		public override void Reset()
		{
			this.Target = null;
			this.SetEnabled = null;
		}

		// Token: 0x06007E87 RID: 32391 RVA: 0x002593E4 File Offset: 0x002575E4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				ProjectileVelocityManager component = safe.GetComponent<ProjectileVelocityManager>();
				if (component)
				{
					component.enabled = this.SetEnabled.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007E45 RID: 32325
		public FsmOwnerDefault Target;

		// Token: 0x04007E46 RID: 32326
		public FsmBool SetEnabled;
	}
}
