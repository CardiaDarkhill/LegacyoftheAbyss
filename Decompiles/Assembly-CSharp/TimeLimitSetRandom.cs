using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class TimeLimitSetRandom : FsmStateAction
{
	// Token: 0x06000196 RID: 406 RVA: 0x00008B73 File Offset: 0x00006D73
	public override void Reset()
	{
		this.MinDelay = null;
		this.MaxDelay = null;
		this.StoreValue = null;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00008B8A File Offset: 0x00006D8A
	public override void OnEnter()
	{
		this.StoreValue.Value = Time.time + Random.Range(this.MinDelay.Value, this.MaxDelay.Value);
		base.Finish();
	}

	// Token: 0x0400016A RID: 362
	public FsmFloat MinDelay;

	// Token: 0x0400016B RID: 363
	public FsmFloat MaxDelay;

	// Token: 0x0400016C RID: 364
	[UIHint(UIHint.Variable)]
	public FsmFloat StoreValue;
}
