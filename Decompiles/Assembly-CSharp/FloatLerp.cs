using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class FloatLerp : FsmStateAction
{
	// Token: 0x0600014A RID: 330 RVA: 0x00007C40 File Offset: 0x00005E40
	public override void Reset()
	{
		this.StartValue = null;
		this.EndValue = null;
		this.Time = null;
		this.StoreValue = null;
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00007C5E File Offset: 0x00005E5E
	public override void OnEnter()
	{
		this.StoreValue.Value = Mathf.Lerp(this.StartValue.Value, this.EndValue.Value, this.Time.Value);
		base.Finish();
	}

	// Token: 0x040000EE RID: 238
	public FsmFloat StartValue;

	// Token: 0x040000EF RID: 239
	public FsmFloat EndValue;

	// Token: 0x040000F0 RID: 240
	public FsmFloat Time;

	// Token: 0x040000F1 RID: 241
	[UIHint(UIHint.Variable)]
	public FsmFloat StoreValue;
}
