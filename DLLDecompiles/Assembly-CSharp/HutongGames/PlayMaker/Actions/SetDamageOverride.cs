using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012BB RID: 4795
	[ActionCategory("Hollow Knight")]
	public class SetDamageOverride : FsmStateAction
	{
		// Token: 0x06007D7B RID: 32123 RVA: 0x0025681F File Offset: 0x00254A1F
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.damageOverride = new FsmBool();
		}

		// Token: 0x06007D7C RID: 32124 RVA: 0x00256838 File Offset: 0x00254A38
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetDamageOverride(this.damageOverride.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D70 RID: 32112
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D71 RID: 32113
		public FsmBool damageOverride;
	}
}
