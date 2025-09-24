using System;
using HutongGames.PlayMaker;

// Token: 0x02000427 RID: 1063
public class SaveGame : FsmStateAction
{
	// Token: 0x06002508 RID: 9480 RVA: 0x000AA988 File Offset: 0x000A8B88
	public override void OnEnter()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.SaveGame(null);
		}
		base.Finish();
	}
}
