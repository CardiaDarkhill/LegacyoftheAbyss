using System;
using HutongGames.PlayMaker;

// Token: 0x02000435 RID: 1077
public sealed class ShopClosedGameMapUpdate : FsmStateAction
{
	// Token: 0x0600252D RID: 9517 RVA: 0x000AAD5B File Offset: 0x000A8F5B
	public override void Reset()
	{
		this.didUpdateEvent = null;
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x000AAD64 File Offset: 0x000A8F64
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			instance.DoShopCloseGameMapUpdate();
			base.Fsm.Event(this.didUpdateEvent);
		}
		base.Finish();
	}

	// Token: 0x040022F4 RID: 8948
	public FsmEvent didUpdateEvent;
}
