using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B7 RID: 4791
	[ActionCategory("Hollow Knight")]
	public class SetGeoDrop : FsmStateAction
	{
		// Token: 0x06007D6F RID: 32111 RVA: 0x002565D6 File Offset: 0x002547D6
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.smallGeo = new FsmInt();
			this.mediumGeo = new FsmInt();
			this.largeGeo = new FsmInt();
		}

		// Token: 0x06007D70 RID: 32112 RVA: 0x00256604 File Offset: 0x00254804
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (!this.smallGeo.IsNone)
					{
						component.SetGeoSmall(this.smallGeo.Value);
					}
					if (!this.mediumGeo.IsNone)
					{
						component.SetGeoMedium(this.mediumGeo.Value);
					}
					if (!this.largeGeo.IsNone)
					{
						component.SetGeoLarge(this.largeGeo.Value);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007D66 RID: 32102
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D67 RID: 32103
		public FsmInt smallGeo;

		// Token: 0x04007D68 RID: 32104
		public FsmInt mediumGeo;

		// Token: 0x04007D69 RID: 32105
		public FsmInt largeGeo;
	}
}
