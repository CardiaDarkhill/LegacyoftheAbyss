using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E6 RID: 4838
	public class GetIsInteractableActive : FsmStateAction
	{
		// Token: 0x06007E2A RID: 32298 RVA: 0x002585A2 File Offset: 0x002567A2
		public override void Reset()
		{
			this.Target = null;
			this.StoreIsActive = null;
		}

		// Token: 0x06007E2B RID: 32299 RVA: 0x002585B4 File Offset: 0x002567B4
		public override void OnEnter()
		{
			this.StoreIsActive.Value = false;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				InteractableBase component = safe.GetComponent<InteractableBase>();
				if (component)
				{
					this.StoreIsActive.Value = !component.IsDisabled;
				}
			}
			base.Finish();
		}

		// Token: 0x04007E00 RID: 32256
		[RequiredField]
		[CheckForComponent(typeof(InteractableBase))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E01 RID: 32257
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsActive;
	}
}
