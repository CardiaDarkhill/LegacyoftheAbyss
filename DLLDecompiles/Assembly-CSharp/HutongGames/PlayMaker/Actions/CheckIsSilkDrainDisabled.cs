using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001230 RID: 4656
	public class CheckIsSilkDrainDisabled : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x06007B57 RID: 31575 RVA: 0x0024F606 File Offset: 0x0024D806
		public override bool IsTrue
		{
			get
			{
				return CheatManager.IsSilkDrainDisabled;
			}
		}
	}
}
