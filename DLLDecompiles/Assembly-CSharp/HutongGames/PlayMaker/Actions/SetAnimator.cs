using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB0 RID: 2992
	public class SetAnimator : FsmStateAction
	{
		// Token: 0x06005C4C RID: 23628 RVA: 0x001D13B7 File Offset: 0x001CF5B7
		public override void Reset()
		{
			this.target = null;
			this.active = null;
		}

		// Token: 0x06005C4D RID: 23629 RVA: 0x001D13C8 File Offset: 0x001CF5C8
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe)
			{
				Animator component = safe.GetComponent<Animator>();
				if (component)
				{
					component.enabled = this.active.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x040057C7 RID: 22471
		public FsmOwnerDefault target;

		// Token: 0x040057C8 RID: 22472
		public FsmBool active;
	}
}
