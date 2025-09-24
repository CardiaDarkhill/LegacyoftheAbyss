using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012BA RID: 4794
	[ActionCategory("Hollow Knight")]
	public class SetHealthManagerReset : FsmStateAction
	{
		// Token: 0x06007D78 RID: 32120 RVA: 0x0025679B File Offset: 0x0025499B
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.reset = new FsmBool();
		}

		// Token: 0x06007D79 RID: 32121 RVA: 0x002567B4 File Offset: 0x002549B4
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.deathReset = this.reset.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D6E RID: 32110
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D6F RID: 32111
		public FsmBool reset;
	}
}
