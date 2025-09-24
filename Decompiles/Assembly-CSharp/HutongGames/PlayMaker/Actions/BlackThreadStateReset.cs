using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200129F RID: 4767
	public class BlackThreadStateReset : FsmStateAction
	{
		// Token: 0x06007D19 RID: 32025 RVA: 0x0025571D File Offset: 0x0025391D
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007D1A RID: 32026 RVA: 0x00255728 File Offset: 0x00253928
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				BlackThreadState componentInParent = safe.GetComponentInParent<BlackThreadState>(true);
				if (componentInParent)
				{
					componentInParent.ResetThreaded();
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

		// Token: 0x04007D26 RID: 32038
		public FsmOwnerDefault Target;
	}
}
