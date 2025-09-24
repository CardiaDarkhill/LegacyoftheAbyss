using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001258 RID: 4696
	public class HeroRegenGainHealth : FsmStateAction
	{
		// Token: 0x06007C05 RID: 31749 RVA: 0x002513EF File Offset: 0x0024F5EF
		public override void Reset()
		{
			this.ExtraHealthAmount = null;
		}

		// Token: 0x06007C06 RID: 31750 RVA: 0x002513F8 File Offset: 0x0024F5F8
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			instance.StartCoroutine(HeroRegenGainHealth.GainHealthOverTime(this.ExtraHealthAmount.Value, instance));
			base.Finish();
		}

		// Token: 0x06007C07 RID: 31751 RVA: 0x00251429 File Offset: 0x0024F629
		private static IEnumerator GainHealthOverTime(int healthAmount, HeroController hc)
		{
			PlayerData pd = hc.playerData;
			WaitForSeconds wait = new WaitForSeconds(0.2f);
			int addedAmount = 0;
			while (addedAmount < healthAmount && pd.health < pd.CurrentMaxHealth)
			{
				hc.AddHealth(1);
				int num = addedAmount;
				addedAmount = num + 1;
				yield return wait;
			}
			yield break;
		}

		// Token: 0x04007C26 RID: 31782
		public FsmInt ExtraHealthAmount;
	}
}
