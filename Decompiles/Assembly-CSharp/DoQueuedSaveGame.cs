using System;
using HutongGames.PlayMaker;

// Token: 0x02000429 RID: 1065
public class DoQueuedSaveGame : FsmStateAction
{
	// Token: 0x0600250C RID: 9484 RVA: 0x000AA9E8 File Offset: 0x000A8BE8
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.DoQueuedSaveGame();
		}
		base.Finish();
	}
}
