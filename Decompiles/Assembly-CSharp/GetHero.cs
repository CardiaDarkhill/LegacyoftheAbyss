using System;
using HutongGames.PlayMaker;

// Token: 0x02000025 RID: 37
[ActionCategory("Hollow Knight")]
public class GetHero : FsmStateAction
{
	// Token: 0x0600014D RID: 333 RVA: 0x00007C9F File Offset: 0x00005E9F
	public override void Reset()
	{
		base.Reset();
		this.storeResult = new FsmGameObject();
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00007CB4 File Offset: 0x00005EB4
	public override void OnEnter()
	{
		base.OnEnter();
		HeroController instance = HeroController.instance;
		this.storeResult.Value = ((instance != null) ? instance.gameObject : null);
		base.Finish();
	}

	// Token: 0x040000F2 RID: 242
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeResult;
}
