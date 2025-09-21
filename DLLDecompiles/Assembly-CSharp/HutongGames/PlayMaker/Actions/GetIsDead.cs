using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B5 RID: 4789
	[ActionCategory("Hollow Knight")]
	public class GetIsDead : FsmStateAction
	{
		// Token: 0x06007D69 RID: 32105 RVA: 0x002564EA File Offset: 0x002546EA
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeValue = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06007D6A RID: 32106 RVA: 0x0025650C File Offset: 0x0025470C
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !this.storeValue.IsNone)
				{
					this.storeValue.Value = component.GetIsDead();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D62 RID: 32098
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D63 RID: 32099
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;
	}
}
