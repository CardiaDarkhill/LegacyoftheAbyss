using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A3 RID: 4771
	[ActionCategory("Hollow Knight")]
	public class SetDamageHero : FsmStateAction
	{
		// Token: 0x06007D28 RID: 32040 RVA: 0x00255917 File Offset: 0x00253B17
		public override void Reset()
		{
			this.Target = null;
			this.Enabled = null;
		}

		// Token: 0x06007D29 RID: 32041 RVA: 0x00255928 File Offset: 0x00253B28
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				DamageHero component = safe.GetComponent<DamageHero>();
				if (component != null)
				{
					component.enabled = this.Enabled.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D2D RID: 32045
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		// Token: 0x04007D2E RID: 32046
		[RequiredField]
		public new FsmBool Enabled;
	}
}
