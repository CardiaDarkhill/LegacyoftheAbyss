using System;
using HutongGames.PlayMaker;

// Token: 0x02000382 RID: 898
[ActionCategory("Hollow Knight/GG")]
public class GGWaitForBossSceneTransitionIn : FsmStateAction
{
	// Token: 0x06001E9B RID: 7835 RVA: 0x0008C6F0 File Offset: 0x0008A8F0
	public override void Reset()
	{
		this.finishEvent = null;
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x0008C6F9 File Offset: 0x0008A8F9
	public override void OnEnter()
	{
		this.DoCheck();
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x0008C701 File Offset: 0x0008A901
	public override void OnUpdate()
	{
		this.DoCheck();
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x0008C709 File Offset: 0x0008A909
	private void DoCheck()
	{
		if (BossSceneController.Instance)
		{
			if (BossSceneController.Instance.HasTransitionedIn)
			{
				base.Fsm.Event(this.finishEvent);
				return;
			}
		}
		else
		{
			base.Fsm.Event(this.finishEvent);
		}
	}

	// Token: 0x04001D95 RID: 7573
	public FsmEvent finishEvent;
}
