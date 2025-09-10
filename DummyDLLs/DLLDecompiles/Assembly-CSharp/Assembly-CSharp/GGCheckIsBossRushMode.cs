using System;
using HutongGames.PlayMaker;

// Token: 0x02000393 RID: 915
[ActionCategory("Hollow Knight/GG")]
public class GGCheckIsBossRushMode : FSMUtility.CheckFsmStateAction
{
	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001EEF RID: 7919 RVA: 0x0008D4E7 File Offset: 0x0008B6E7
	public override bool IsTrue
	{
		get
		{
			return GameManager.instance.playerData.bossRushMode;
		}
	}
}
