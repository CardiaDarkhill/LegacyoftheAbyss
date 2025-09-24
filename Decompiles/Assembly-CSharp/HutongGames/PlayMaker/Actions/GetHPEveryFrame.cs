using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B4 RID: 4788
	[ActionCategory("Hollow Knight")]
	public class GetHPEveryFrame : FsmStateAction
	{
		// Token: 0x06007D65 RID: 32101 RVA: 0x0025645E File Offset: 0x0025465E
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeValue = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x06007D66 RID: 32102 RVA: 0x00256480 File Offset: 0x00254680
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				this.healthManager = safe.GetComponent<HealthManager>();
			}
		}

		// Token: 0x06007D67 RID: 32103 RVA: 0x002564AF File Offset: 0x002546AF
		public override void OnUpdate()
		{
			if (this.healthManager != null && !this.storeValue.IsNone)
			{
				this.storeValue.Value = this.healthManager.hp;
			}
		}

		// Token: 0x04007D5F RID: 32095
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D60 RID: 32096
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;

		// Token: 0x04007D61 RID: 32097
		private HealthManager healthManager;
	}
}
