using System;
using HutongGames.PlayMaker;

// Token: 0x0200037F RID: 895
[ActionCategory("Hollow Knight/GG")]
public class CheckGGBossLevel : FsmStateAction
{
	// Token: 0x06001E93 RID: 7827 RVA: 0x0008C5D5 File Offset: 0x0008A7D5
	public override void Reset()
	{
		this.notGG = null;
		this.level1 = null;
		this.level2 = null;
		this.level3 = null;
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x0008C5F4 File Offset: 0x0008A7F4
	public override void OnEnter()
	{
		if (BossSceneController.Instance)
		{
			switch (BossSceneController.Instance.BossLevel)
			{
			case 0:
				base.Fsm.Event(this.level1);
				break;
			case 1:
				base.Fsm.Event(this.level2);
				break;
			case 2:
				base.Fsm.Event(this.level3);
				break;
			}
		}
		else
		{
			base.Fsm.Event(this.notGG);
		}
		base.Finish();
	}

	// Token: 0x04001D8F RID: 7567
	public FsmEvent notGG;

	// Token: 0x04001D90 RID: 7568
	public FsmEvent level1;

	// Token: 0x04001D91 RID: 7569
	public FsmEvent level2;

	// Token: 0x04001D92 RID: 7570
	public FsmEvent level3;
}
