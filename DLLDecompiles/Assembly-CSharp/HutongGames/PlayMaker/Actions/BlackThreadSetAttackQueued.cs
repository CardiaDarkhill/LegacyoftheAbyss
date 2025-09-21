using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200129E RID: 4766
	public class BlackThreadSetAttackQueued : FsmStateAction
	{
		// Token: 0x06007D16 RID: 32022 RVA: 0x00255683 File Offset: 0x00253883
		public override void Reset()
		{
			this.Target = null;
			this.SetAttackQueued = true;
		}

		// Token: 0x06007D17 RID: 32023 RVA: 0x00255698 File Offset: 0x00253898
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				BlackThreadState componentInParent = safe.GetComponentInParent<BlackThreadState>(true);
				if (componentInParent)
				{
					componentInParent.SetAttackQueued(this.SetAttackQueued.Value);
				}
				else
				{
					Debug.LogError("Object \"" + safe.name + "\" does not have a BlackThreadState component", base.Owner);
				}
			}
			else
			{
				Debug.LogError("Target is null", base.Owner);
			}
			base.Finish();
		}

		// Token: 0x04007D24 RID: 32036
		public FsmOwnerDefault Target;

		// Token: 0x04007D25 RID: 32037
		public FsmBool SetAttackQueued;
	}
}
