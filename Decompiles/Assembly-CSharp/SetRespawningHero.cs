using System;
using HutongGames.PlayMaker;

// Token: 0x0200041C RID: 1052
[ActionCategory("Hollow Knight")]
public class SetRespawningHero : FsmStateAction
{
	// Token: 0x060024E7 RID: 9447 RVA: 0x000AA365 File Offset: 0x000A8565
	public override void Reset()
	{
		this.value = null;
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x000AA36E File Offset: 0x000A856E
	public override void OnEnter()
	{
		if (GameManager.instance)
		{
			GameManager.instance.RespawningHero = this.value.Value;
		}
		base.Finish();
	}

	// Token: 0x040022C9 RID: 8905
	public FsmBool value;
}
