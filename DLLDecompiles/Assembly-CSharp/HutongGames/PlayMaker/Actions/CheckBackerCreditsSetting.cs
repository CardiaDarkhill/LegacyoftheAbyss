using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D9 RID: 4825
	public class CheckBackerCreditsSetting : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C1B RID: 3099
		// (get) Token: 0x06007DDD RID: 32221 RVA: 0x00257773 File Offset: 0x00255973
		public override bool IsTrue
		{
			get
			{
				return GameManager.instance.gameSettings.backerCredits != 0;
			}
		}
	}
}
