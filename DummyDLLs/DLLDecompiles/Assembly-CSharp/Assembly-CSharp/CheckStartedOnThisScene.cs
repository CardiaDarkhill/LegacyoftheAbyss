using System;

// Token: 0x02000434 RID: 1076
public class CheckStartedOnThisScene : FSMUtility.CheckFsmStateAction
{
	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x0600252B RID: 9515 RVA: 0x000AAD47 File Offset: 0x000A8F47
	public override bool IsTrue
	{
		get
		{
			return GameManager.instance.startedOnThisScene;
		}
	}
}
