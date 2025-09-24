using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001200 RID: 4608
	public class CheckIsClamberBlocked : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x06007AA6 RID: 31398 RVA: 0x0024CFFA File Offset: 0x0024B1FA
		public override bool IsTrue
		{
			get
			{
				return NoClamberRegion.IsClamberBlocked;
			}
		}
	}
}
