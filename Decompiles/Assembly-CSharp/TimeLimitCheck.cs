using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class TimeLimitCheck : FsmStateAction
{
	// Token: 0x0600018C RID: 396 RVA: 0x00008A5A File Offset: 0x00006C5A
	public override void Reset()
	{
		this.storedValue = null;
		this.aboveEvent = null;
		this.belowEvent = null;
		this.EveryFrame = false;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00008A78 File Offset: 0x00006C78
	public override void OnEnter()
	{
		this.DoAction();
		if (!this.EveryFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00008A8E File Offset: 0x00006C8E
	public override void OnUpdate()
	{
		this.DoAction();
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00008A96 File Offset: 0x00006C96
	private void DoAction()
	{
		base.Fsm.Event((Time.time >= this.storedValue.Value) ? this.aboveEvent : this.belowEvent);
	}

	// Token: 0x04000161 RID: 353
	[UIHint(UIHint.Variable)]
	public FsmFloat storedValue;

	// Token: 0x04000162 RID: 354
	public FsmEvent aboveEvent;

	// Token: 0x04000163 RID: 355
	public FsmEvent belowEvent;

	// Token: 0x04000164 RID: 356
	public bool EveryFrame;
}
