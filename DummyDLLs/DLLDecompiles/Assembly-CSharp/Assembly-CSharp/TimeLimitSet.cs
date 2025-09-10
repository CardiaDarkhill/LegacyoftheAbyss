using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class TimeLimitSet : FsmStateAction
{
	// Token: 0x06000184 RID: 388 RVA: 0x00008997 File Offset: 0x00006B97
	public override void Reset()
	{
		this.timeDelay = null;
		this.storeValue = null;
	}

	// Token: 0x06000185 RID: 389 RVA: 0x000089A7 File Offset: 0x00006BA7
	public override void OnEnter()
	{
		this.storeValue.Value = Time.time + this.timeDelay.Value;
		base.Finish();
	}

	// Token: 0x0400015B RID: 347
	public FsmFloat timeDelay;

	// Token: 0x0400015C RID: 348
	[UIHint(UIHint.Variable)]
	public FsmFloat storeValue;
}
