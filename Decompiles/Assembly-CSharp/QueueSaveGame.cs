using System;
using HutongGames.PlayMaker;

// Token: 0x02000428 RID: 1064
public class QueueSaveGame : FsmStateAction
{
	// Token: 0x0600250A RID: 9482 RVA: 0x000AA9B8 File Offset: 0x000A8BB8
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.QueueSaveGame();
		}
		base.Finish();
	}
}
