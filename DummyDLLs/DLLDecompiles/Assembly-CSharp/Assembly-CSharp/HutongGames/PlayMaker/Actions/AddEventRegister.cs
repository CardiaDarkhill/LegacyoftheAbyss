using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D5 RID: 4821
	[ActionCategory("Hollow Knight")]
	public class AddEventRegister : FsmStateAction
	{
		// Token: 0x06007DCE RID: 32206 RVA: 0x00257569 File Offset: 0x00255769
		public override void Reset()
		{
			this.eventName = new FsmString();
		}

		// Token: 0x06007DCF RID: 32207 RVA: 0x00257578 File Offset: 0x00255778
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.eventName.Value))
			{
				GameObject safe = this.target.GetSafe(this);
				if (safe)
				{
					EventRegister.GetRegisterGuaranteed(safe, this.eventName.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007DB2 RID: 32178
		public FsmOwnerDefault target;

		// Token: 0x04007DB3 RID: 32179
		public FsmString eventName;
	}
}
