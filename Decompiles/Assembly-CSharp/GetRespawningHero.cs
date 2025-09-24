using System;
using HutongGames.PlayMaker;

// Token: 0x0200041B RID: 1051
[ActionCategory("Hollow Knight")]
public class GetRespawningHero : FsmStateAction
{
	// Token: 0x060024E4 RID: 9444 RVA: 0x000AA327 File Offset: 0x000A8527
	public override void Reset()
	{
		this.variable = new FsmBool();
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x000AA334 File Offset: 0x000A8534
	public override void OnEnter()
	{
		if (GameManager.instance)
		{
			this.variable.Value = GameManager.instance.RespawningHero;
		}
		base.Finish();
	}

	// Token: 0x040022C8 RID: 8904
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmBool variable;
}
