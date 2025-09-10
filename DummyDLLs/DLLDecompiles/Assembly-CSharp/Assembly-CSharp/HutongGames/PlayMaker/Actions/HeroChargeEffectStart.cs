using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001235 RID: 4661
	public class HeroChargeEffectStart : FsmStateAction
	{
		// Token: 0x06007B6D RID: 31597 RVA: 0x0024F99B File Offset: 0x0024DB9B
		public override void Reset()
		{
			this.TintColor = Color.white;
		}

		// Token: 0x06007B6E RID: 31598 RVA: 0x0024F9AD File Offset: 0x0024DBAD
		public override void OnEnter()
		{
			ManagerSingleton<HeroChargeEffects>.Instance.StartCharge(this.TintColor.Value);
			base.Finish();
		}

		// Token: 0x04007BAC RID: 31660
		public FsmColor TintColor;
	}
}
