using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001345 RID: 4933
	public class CheckIsAnyCurrencyCounterRolling : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x06007F77 RID: 32631 RVA: 0x0025B7EF File Offset: 0x002599EF
		public override bool IsTrue
		{
			get
			{
				return CurrencyCounterBase.IsAnyCounterRolling;
			}
		}
	}
}
