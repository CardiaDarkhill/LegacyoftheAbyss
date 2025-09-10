using System;
using HutongGames.PlayMaker;

// Token: 0x0200037E RID: 894
[ActionCategory("Hollow Knight/GG")]
public class EndGGBossScene : FsmStateAction
{
	// Token: 0x06001E91 RID: 7825 RVA: 0x0008C5AF File Offset: 0x0008A7AF
	public override void OnEnter()
	{
		if (BossSceneController.Instance)
		{
			BossSceneController.Instance.EndBossScene();
		}
		base.Finish();
	}
}
