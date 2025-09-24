using System;

// Token: 0x0200042E RID: 1070
public class CheckIsFirstLevelForPlayer : FSMUtility.CheckFsmStateAction
{
	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x0600251A RID: 9498 RVA: 0x000AAB80 File Offset: 0x000A8D80
	public override bool IsTrue
	{
		get
		{
			return GameManager.instance.IsFirstLevelForPlayer;
		}
	}
}
