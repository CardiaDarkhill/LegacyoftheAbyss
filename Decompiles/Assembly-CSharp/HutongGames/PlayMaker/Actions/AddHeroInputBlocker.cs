using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001259 RID: 4697
	public class AddHeroInputBlocker : FsmStateAction
	{
		// Token: 0x06007C09 RID: 31753 RVA: 0x00251447 File Offset: 0x0024F647
		public override void Reset()
		{
			this.Blocker = null;
		}

		// Token: 0x06007C0A RID: 31754 RVA: 0x00251450 File Offset: 0x0024F650
		public override void OnEnter()
		{
			GameObject safe = this.Blocker.GetSafe(this);
			if (safe)
			{
				HeroController instance = HeroController.instance;
				if (instance)
				{
					instance.AddInputBlocker(safe);
				}
			}
			base.Finish();
		}

		// Token: 0x04007C27 RID: 31783
		public FsmOwnerDefault Blocker;
	}
}
