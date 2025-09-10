using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001257 RID: 4695
	public class HeroRegenGainSilk : FsmStateAction
	{
		// Token: 0x06007C01 RID: 31745 RVA: 0x00251397 File Offset: 0x0024F597
		public override void Reset()
		{
			this.ExtraSilkAmount = null;
		}

		// Token: 0x06007C02 RID: 31746 RVA: 0x002513A0 File Offset: 0x0024F5A0
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			instance.StartCoroutine(HeroRegenGainSilk.GainSilkOverTime(this.ExtraSilkAmount.Value, instance));
			base.Finish();
		}

		// Token: 0x06007C03 RID: 31747 RVA: 0x002513D1 File Offset: 0x0024F5D1
		private static IEnumerator GainSilkOverTime(int silkAmount, HeroController hc)
		{
			PlayerData pd = hc.playerData;
			int num = pd.CurrentSilkRegenMax - pd.silk;
			if (num > 0)
			{
				silkAmount += num;
			}
			WaitForSeconds wait = new WaitForSeconds(0.1f);
			int addedAmount = 0;
			while (addedAmount < silkAmount && pd.silk < pd.CurrentSilkMax)
			{
				hc.AddSilk(1, false);
				int num2 = addedAmount;
				addedAmount = num2 + 1;
				yield return wait;
			}
			yield break;
		}

		// Token: 0x04007C25 RID: 31781
		public FsmInt ExtraSilkAmount;
	}
}
