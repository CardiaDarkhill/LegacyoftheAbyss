using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A4 RID: 4772
	[ActionCategory("Hollow Knight")]
	public class SetDamageHeroOnExit : FsmStateAction
	{
		// Token: 0x06007D2B RID: 32043 RVA: 0x0025597A File Offset: 0x00253B7A
		public override void Reset()
		{
			this.Target = null;
			this.Enabled = null;
		}

		// Token: 0x06007D2C RID: 32044 RVA: 0x0025598C File Offset: 0x00253B8C
		public override void OnExit()
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

		// Token: 0x04007D2F RID: 32047
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		// Token: 0x04007D30 RID: 32048
		[RequiredField]
		public new FsmBool Enabled;
	}
}
