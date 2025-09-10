using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class CheckLineOfSight : FsmStateAction
{
	// Token: 0x06001BA2 RID: 7074 RVA: 0x00080DF5 File Offset: 0x0007EFF5
	public override void Reset()
	{
		this.From = null;
		this.To = null;
		this.blockingLayerMask = 8;
		this.storeResult = null;
		this.inSightEvent = null;
		this.noSightEvent = null;
		this.everyFrame = false;
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x00080E34 File Offset: 0x0007F034
	public override void OnEnter()
	{
		this.from = this.From.GetSafe(this);
		this.isValid = (this.from != null);
		if (this.isValid)
		{
			this.to = this.To.Value;
			this.isValid = (this.to != null);
			bool flag = this.isValid;
		}
		this.DoCheck();
		if (!this.isValid || !this.everyFrame.Value)
		{
			base.Finish();
		}
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x00080EB8 File Offset: 0x0007F0B8
	private void DoCheck()
	{
		bool flag = false;
		if (this.isValid)
		{
			flag = !Physics2D.Linecast(this.from.transform.position, this.to.transform.position, this.blockingLayerMask.Value);
		}
		this.storeResult.Value = flag;
		if (flag)
		{
			this.Event(this.inSightEvent);
			return;
		}
		this.Event(this.noSightEvent);
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x00080F3C File Offset: 0x0007F13C
	public override void OnUpdate()
	{
		if (!this.isValid)
		{
			base.Finish();
			return;
		}
		this.DoCheck();
	}

	// Token: 0x04001AA5 RID: 6821
	public FsmOwnerDefault From;

	// Token: 0x04001AA6 RID: 6822
	public FsmGameObject To;

	// Token: 0x04001AA7 RID: 6823
	[UIHint(UIHint.LayerMask)]
	public FsmInt blockingLayerMask;

	// Token: 0x04001AA8 RID: 6824
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	// Token: 0x04001AA9 RID: 6825
	public FsmEvent inSightEvent;

	// Token: 0x04001AAA RID: 6826
	public FsmEvent noSightEvent;

	// Token: 0x04001AAB RID: 6827
	public FsmBool everyFrame;

	// Token: 0x04001AAC RID: 6828
	private bool isValid;

	// Token: 0x04001AAD RID: 6829
	private GameObject from;

	// Token: 0x04001AAE RID: 6830
	private GameObject to;
}
