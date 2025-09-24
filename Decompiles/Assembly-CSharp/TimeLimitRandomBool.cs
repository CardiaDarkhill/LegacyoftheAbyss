using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class TimeLimitRandomBool : FsmStateAction
{
	// Token: 0x06000199 RID: 409 RVA: 0x00008BC6 File Offset: 0x00006DC6
	public override void Reset()
	{
		this.MinDelay = null;
		this.MaxDelay = null;
		this.StoreValue = null;
		this.AboveValue = true;
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00008BEC File Offset: 0x00006DEC
	public override void OnEnter()
	{
		float num = Random.Range(this.MinDelay.Value, this.MaxDelay.Value);
		if (num <= 0f)
		{
			this.StoreValue.Value = this.AboveValue.Value;
			base.Finish();
			return;
		}
		this.time = Time.time + num;
		this.StoreValue.Value = !this.AboveValue.Value;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00008C60 File Offset: 0x00006E60
	public override void OnUpdate()
	{
		if (Time.time >= this.time)
		{
			this.StoreValue.Value = this.AboveValue.Value;
			base.Finish();
			return;
		}
		this.StoreValue.Value = !this.AboveValue.Value;
	}

	// Token: 0x0400016D RID: 365
	public FsmFloat MinDelay;

	// Token: 0x0400016E RID: 366
	public FsmFloat MaxDelay;

	// Token: 0x0400016F RID: 367
	[UIHint(UIHint.Variable)]
	public FsmBool StoreValue;

	// Token: 0x04000170 RID: 368
	public FsmBool AboveValue;

	// Token: 0x04000171 RID: 369
	private float time;
}
