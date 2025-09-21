using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E9 RID: 4841
	public class EndInteractEvents : FsmStateAction
	{
		// Token: 0x06007E35 RID: 32309 RVA: 0x00258795 File Offset: 0x00256995
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007E36 RID: 32310 RVA: 0x002587A0 File Offset: 0x002569A0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				InteractEvents component = safe.GetComponent<InteractEvents>();
				if (component)
				{
					component.EndInteraction();
				}
			}
			base.Finish();
		}

		// Token: 0x04007E0B RID: 32267
		[RequiredField]
		[CheckForComponent(typeof(InteractEvents))]
		public FsmOwnerDefault Target;
	}
}
