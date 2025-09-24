using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class TimeLimitSetV2 : FsmStateAction
{
	// Token: 0x06000187 RID: 391 RVA: 0x000089D3 File Offset: 0x00006BD3
	public override void Reset()
	{
		this.TimeDelay = null;
		this.StoreValue = null;
		this.CanSet = new FsmBool
		{
			UseVariable = true
		};
		this.EveryFrame = false;
	}

	// Token: 0x06000188 RID: 392 RVA: 0x000089FC File Offset: 0x00006BFC
	public override void OnEnter()
	{
		this.DoAction();
		if (!this.EveryFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00008A12 File Offset: 0x00006C12
	public override void OnUpdate()
	{
		this.DoAction();
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00008A1A File Offset: 0x00006C1A
	private void DoAction()
	{
		if (this.CanSet.IsNone || this.CanSet.Value)
		{
			this.StoreValue.Value = Time.time + this.TimeDelay.Value;
		}
	}

	// Token: 0x0400015D RID: 349
	public FsmFloat TimeDelay;

	// Token: 0x0400015E RID: 350
	[UIHint(UIHint.Variable)]
	public FsmFloat StoreValue;

	// Token: 0x0400015F RID: 351
	public FsmBool CanSet;

	// Token: 0x04000160 RID: 352
	public bool EveryFrame;
}
