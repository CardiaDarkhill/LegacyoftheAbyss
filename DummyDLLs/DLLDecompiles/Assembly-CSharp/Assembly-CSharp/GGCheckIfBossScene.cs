using System;
using HutongGames.PlayMaker;

// Token: 0x02000380 RID: 896
[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossScene : FsmStateAction
{
	// Token: 0x06001E96 RID: 7830 RVA: 0x0008C684 File Offset: 0x0008A884
	public override void Reset()
	{
		this.bossSceneEvent = null;
		this.regularSceneEvent = null;
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x0008C694 File Offset: 0x0008A894
	public override void OnEnter()
	{
		if (BossSceneController.IsBossScene)
		{
			base.Fsm.Event(this.bossSceneEvent);
		}
		else
		{
			base.Fsm.Event(this.regularSceneEvent);
		}
		base.Finish();
	}

	// Token: 0x04001D93 RID: 7571
	public FsmEvent bossSceneEvent;

	// Token: 0x04001D94 RID: 7572
	public FsmEvent regularSceneEvent;
}
