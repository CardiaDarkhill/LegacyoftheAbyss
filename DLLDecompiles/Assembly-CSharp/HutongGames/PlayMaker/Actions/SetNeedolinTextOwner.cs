using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200130C RID: 4876
	public class SetNeedolinTextOwner : FsmStateAction
	{
		// Token: 0x06007EAD RID: 32429 RVA: 0x00259898 File Offset: 0x00257A98
		public override void Reset()
		{
			this.Target = null;
			this.SetEnabled = null;
		}

		// Token: 0x06007EAE RID: 32430 RVA: 0x002598A8 File Offset: 0x00257AA8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				NeedolinTextOwner component = safe.GetComponent<NeedolinTextOwner>();
				if (component)
				{
					component.enabled = this.SetEnabled.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007E61 RID: 32353
		[RequiredField]
		[CheckForComponent(typeof(NeedolinTextOwner))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E62 RID: 32354
		public FsmBool SetEnabled;
	}
}
