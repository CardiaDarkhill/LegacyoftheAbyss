using System;
using HutongGames.PlayMaker;

// Token: 0x02000419 RID: 1049
[ActionCategory("Hollow Knight")]
public class WaitForBossLoad : FsmStateAction
{
	// Token: 0x060024DE RID: 9438 RVA: 0x000AA21A File Offset: 0x000A841A
	public override void Reset()
	{
		this.sendEvent = null;
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000AA224 File Offset: 0x000A8424
	public override void OnEnter()
	{
		if (!WorldInfo.NameLooksLikeAdditiveLoadScene(base.Owner.scene.name) && GameManager.instance && SceneAdditiveLoadConditional.ShouldLoadBoss)
		{
			GameManager.BossLoad temp = null;
			temp = delegate()
			{
				this.Fsm.Event(this.sendEvent);
				GameManager.instance.OnLoadedBoss -= temp;
				this.Finish();
			};
			GameManager.instance.OnLoadedBoss += temp;
			return;
		}
		base.Finish();
	}

	// Token: 0x040022C6 RID: 8902
	public FsmEvent sendEvent;
}
