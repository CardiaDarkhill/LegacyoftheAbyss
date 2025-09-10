using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200131A RID: 4890
	public class CheckIsInQuestBoardDepositSequence : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x06007EE2 RID: 32482 RVA: 0x0025A0EE File Offset: 0x002582EE
		public override bool IsTrue
		{
			get
			{
				return StaticVariableList.GetValue<bool>("IsInQuestBoardDepositSequence", false);
			}
		}
	}
}
