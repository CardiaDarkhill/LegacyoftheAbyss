using System;
using HutongGames.PlayMaker;

// Token: 0x02000436 RID: 1078
public sealed class DoGameMapUpdate : FsmStateAction
{
	// Token: 0x06002530 RID: 9520 RVA: 0x000AADA5 File Offset: 0x000A8FA5
	private bool IsSilent()
	{
		return this.silent.Value;
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x000AADB2 File Offset: 0x000A8FB2
	public override void Reset()
	{
		this.silent = null;
		this.delay = null;
		this.didUpdateEvent = null;
	}

	// Token: 0x06002532 RID: 9522 RVA: 0x000AADCC File Offset: 0x000A8FCC
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			bool flag;
			if (this.silent.Value)
			{
				flag = instance.UpdateGameMap();
			}
			else
			{
				flag = instance.UpdateGameMapWithPopup(this.delay.Value);
			}
			if (flag)
			{
				base.Fsm.Event(this.didUpdateEvent);
			}
		}
		base.Finish();
	}

	// Token: 0x040022F5 RID: 8949
	public FsmBool silent;

	// Token: 0x040022F6 RID: 8950
	[HideIf("IsSilent")]
	public FsmFloat delay;

	// Token: 0x040022F7 RID: 8951
	public FsmEvent didUpdateEvent;
}
