using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class TimeLimitCheckV2 : FsmStateAction
{
	// Token: 0x06000191 RID: 401 RVA: 0x00008ACB File Offset: 0x00006CCB
	public override void Reset()
	{
		this.StoredValue = null;
		this.AboveEvent = null;
		this.BelowEvent = null;
		this.StoreBool = null;
		this.EveryFrame = false;
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00008AF0 File Offset: 0x00006CF0
	public override void OnEnter()
	{
		this.DoAction();
		if (!this.EveryFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00008B06 File Offset: 0x00006D06
	public override void OnUpdate()
	{
		this.DoAction();
	}

	// Token: 0x06000194 RID: 404 RVA: 0x00008B10 File Offset: 0x00006D10
	private void DoAction()
	{
		bool flag = Time.time >= this.StoredValue.Value;
		this.StoreBool.Value = flag;
		if (flag)
		{
			base.Fsm.Event(this.AboveEvent);
			base.Finish();
			return;
		}
		base.Fsm.Event(this.BelowEvent);
	}

	// Token: 0x04000165 RID: 357
	[UIHint(UIHint.Variable)]
	public FsmFloat StoredValue;

	// Token: 0x04000166 RID: 358
	public FsmEvent AboveEvent;

	// Token: 0x04000167 RID: 359
	public FsmEvent BelowEvent;

	// Token: 0x04000168 RID: 360
	[UIHint(UIHint.Variable)]
	public FsmBool StoreBool;

	// Token: 0x04000169 RID: 361
	public bool EveryFrame;
}
