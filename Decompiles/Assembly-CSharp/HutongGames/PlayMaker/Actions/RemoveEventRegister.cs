using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D6 RID: 4822
	[ActionCategory("Hollow Knight")]
	public class RemoveEventRegister : FsmStateAction
	{
		// Token: 0x06007DD1 RID: 32209 RVA: 0x002575CC File Offset: 0x002557CC
		public override void Reset()
		{
			this.Target = null;
			this.EventName = null;
		}

		// Token: 0x06007DD2 RID: 32210 RVA: 0x002575DC File Offset: 0x002557DC
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.EventName.Value))
			{
				GameObject safe = this.Target.GetSafe(this);
				if (safe)
				{
					EventRegister.RemoveRegister(safe, this.EventName.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007DB4 RID: 32180
		public FsmOwnerDefault Target;

		// Token: 0x04007DB5 RID: 32181
		public FsmString EventName;
	}
}
