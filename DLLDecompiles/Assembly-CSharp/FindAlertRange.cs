using System;
using HutongGames.PlayMaker;

// Token: 0x02000292 RID: 658
[ActionCategory("Hollow Knight")]
public class FindAlertRange : FsmStateAction
{
	// Token: 0x06001708 RID: 5896 RVA: 0x00068117 File Offset: 0x00066317
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.storeResult = new FsmObject();
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x0006812F File Offset: 0x0006632F
	public override void OnEnter()
	{
		this.storeResult.Value = AlertRange.Find(this.target.GetSafe(this), this.childName);
		base.Finish();
	}

	// Token: 0x04001594 RID: 5524
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x04001595 RID: 5525
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(AlertRange))]
	public FsmObject storeResult;

	// Token: 0x04001596 RID: 5526
	public string childName;
}
