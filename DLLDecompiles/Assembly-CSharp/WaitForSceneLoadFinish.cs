using System;
using HutongGames.PlayMaker;

// Token: 0x02000421 RID: 1057
[ActionCategory("Hollow Knight")]
public class WaitForSceneLoadFinish : FsmStateAction
{
	// Token: 0x060024F6 RID: 9462 RVA: 0x000AA533 File Offset: 0x000A8733
	public override void Reset()
	{
		this.sendEvent = null;
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x000AA53C File Offset: 0x000A873C
	public override void OnEnter()
	{
		if (GameManager.instance && GameManager.instance.IsInSceneTransition)
		{
			GameManager.SceneTransitionFinishEvent temp = null;
			temp = delegate()
			{
				this.Fsm.Event(this.sendEvent);
				GameManager.instance.OnFinishedSceneTransition -= temp;
				this.Finish();
			};
			GameManager.instance.OnFinishedSceneTransition += temp;
			return;
		}
		base.Finish();
	}

	// Token: 0x040022D2 RID: 8914
	[RequiredField]
	public FsmEvent sendEvent;
}
