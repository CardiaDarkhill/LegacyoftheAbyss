using System;
using HutongGames.PlayMaker;

// Token: 0x02000420 RID: 1056
[ActionCategory("Hollow Knight")]
public class DestroyPersonalPools : FsmStateAction
{
	// Token: 0x060024F4 RID: 9460 RVA: 0x000AA50D File Offset: 0x000A870D
	public override void OnEnter()
	{
		if (GameManager.instance)
		{
			GameManager.instance.DoDestroyPersonalPools();
		}
		base.Finish();
	}
}
