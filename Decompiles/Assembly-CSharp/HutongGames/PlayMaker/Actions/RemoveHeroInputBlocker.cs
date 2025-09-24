using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200125A RID: 4698
	public class RemoveHeroInputBlocker : FsmStateAction
	{
		// Token: 0x06007C0C RID: 31756 RVA: 0x00251495 File Offset: 0x0024F695
		public override void Reset()
		{
			this.Blocker = null;
		}

		// Token: 0x06007C0D RID: 31757 RVA: 0x002514A0 File Offset: 0x0024F6A0
		public override void OnEnter()
		{
			GameObject safe = this.Blocker.GetSafe(this);
			if (safe)
			{
				HeroController instance = HeroController.instance;
				if (instance)
				{
					instance.RemoveInputBlocker(safe);
				}
			}
			base.Finish();
		}

		// Token: 0x04007C28 RID: 31784
		public FsmOwnerDefault Blocker;
	}
}
