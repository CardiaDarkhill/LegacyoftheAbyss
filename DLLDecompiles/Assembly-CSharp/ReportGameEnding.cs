using System;
using HutongGames.PlayMaker;

// Token: 0x02000439 RID: 1081
public class ReportGameEnding : FsmStateAction
{
	// Token: 0x0600253B RID: 9531 RVA: 0x000AAF1C File Offset: 0x000A911C
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			instance.RecordGameComplete();
		}
	}
}
