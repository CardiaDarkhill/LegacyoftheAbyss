using System;
using HutongGames.PlayMaker;

// Token: 0x02000430 RID: 1072
public class GetEntryGateName : FsmStateAction
{
	// Token: 0x0600251F RID: 9503 RVA: 0x000AABDB File Offset: 0x000A8DDB
	public override void Reset()
	{
		this.StoreValue = null;
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000AABE4 File Offset: 0x000A8DE4
	public override void OnEnter()
	{
		this.StoreValue.Value = GameManager.instance.GetEntryGateName();
		base.Finish();
	}

	// Token: 0x040022EF RID: 8943
	[UIHint(UIHint.Variable)]
	public FsmString StoreValue;
}
