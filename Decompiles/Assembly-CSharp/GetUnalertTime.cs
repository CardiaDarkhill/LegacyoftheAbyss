using System;
using HutongGames.PlayMaker;

// Token: 0x02000296 RID: 662
[ActionCategory("Hollow Knight")]
public class GetUnalertTime : FsmStateAction
{
	// Token: 0x0600171E RID: 5918 RVA: 0x000685AE File Offset: 0x000667AE
	public override void Reset()
	{
		this.alertRange = new FsmObject
		{
			UseVariable = true
		};
		this.storeTime = new FsmFloat
		{
			UseVariable = true
		};
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x000685D4 File Offset: 0x000667D4
	public override void OnEnter()
	{
		if (this.alertRange.Value == null || this.alertRange.IsNone)
		{
			base.Finish();
		}
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x000685FC File Offset: 0x000667FC
	public override void OnUpdate()
	{
		if (this.alertRange.Value != null)
		{
			AlertRange alertRange = this.alertRange.Value as AlertRange;
			this.storeTime.Value = alertRange.GetUnalertTime();
		}
	}

	// Token: 0x040015AE RID: 5550
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	// Token: 0x040015AF RID: 5551
	[UIHint(UIHint.Variable)]
	public FsmFloat storeTime;
}
