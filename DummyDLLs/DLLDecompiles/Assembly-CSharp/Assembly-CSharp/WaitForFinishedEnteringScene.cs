using System;
using HutongGames.PlayMaker;

// Token: 0x0200041A RID: 1050
[ActionCategory("Hollow Knight")]
public class WaitForFinishedEnteringScene : FsmStateAction
{
	// Token: 0x060024E1 RID: 9441 RVA: 0x000AA2A3 File Offset: 0x000A84A3
	public override void Reset()
	{
		this.sendEvent = null;
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000AA2AC File Offset: 0x000A84AC
	public override void OnEnter()
	{
		if (GameManager.instance && !GameManager.instance.HasFinishedEnteringScene)
		{
			GameManager.EnterSceneEvent temp = null;
			temp = delegate()
			{
				this.Fsm.Event(this.sendEvent);
				GameManager.instance.OnFinishedEnteringScene -= temp;
				this.Finish();
			};
			GameManager.instance.OnFinishedEnteringScene += temp;
			return;
		}
		base.Fsm.Event(this.sendEvent);
		base.Finish();
	}

	// Token: 0x040022C7 RID: 8903
	public FsmEvent sendEvent;
}
