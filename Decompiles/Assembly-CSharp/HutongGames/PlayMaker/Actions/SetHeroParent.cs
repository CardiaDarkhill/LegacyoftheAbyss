using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200125B RID: 4699
	public class SetHeroParent : FsmStateAction
	{
		// Token: 0x06007C0F RID: 31759 RVA: 0x002514E5 File Offset: 0x0024F6E5
		public override void Reset()
		{
			this.NewParent = null;
		}

		// Token: 0x06007C10 RID: 31760 RVA: 0x002514F0 File Offset: 0x0024F6F0
		public override void OnEnter()
		{
			GameObject safe = this.NewParent.GetSafe(this);
			HeroController instance = HeroController.instance;
			if (instance)
			{
				instance.SetHeroParent(safe ? safe.transform : null);
			}
			base.Finish();
		}

		// Token: 0x04007C29 RID: 31785
		public FsmOwnerDefault NewParent;
	}
}
