using System;
using HutongGames.PlayMaker;

// Token: 0x02000297 RID: 663
[ActionCategory("Hollow Knight")]
public class CompareUnalertTime : FsmStateAction
{
	// Token: 0x06001722 RID: 5922 RVA: 0x00068646 File Offset: 0x00066846
	public override void Reset()
	{
		this.alertRange = new FsmObject
		{
			UseVariable = true
		};
		this.compareTo = new FsmFloat();
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x00068665 File Offset: 0x00066865
	public override void OnEnter()
	{
		if (this.alertRange.Value == null || this.alertRange.IsNone)
		{
			base.Finish();
		}
		this.DoCompare();
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x000686A1 File Offset: 0x000668A1
	public override void OnUpdate()
	{
		this.DoCompare();
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x000686AC File Offset: 0x000668AC
	private void DoCompare()
	{
		if (this.alertRange.Value != null)
		{
			if ((this.alertRange.Value as AlertRange).GetUnalertTime() <= this.compareTo.Value)
			{
				base.Fsm.Event(this.lessThanOrEqualEvent);
				this.greatherThanBool.Value = false;
				return;
			}
			base.Fsm.Event(this.greatherThanEvent);
			this.greatherThanBool.Value = true;
		}
	}

	// Token: 0x040015B0 RID: 5552
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	// Token: 0x040015B1 RID: 5553
	public FsmFloat compareTo;

	// Token: 0x040015B2 RID: 5554
	public FsmEvent lessThanOrEqualEvent;

	// Token: 0x040015B3 RID: 5555
	public FsmEvent greatherThanEvent;

	// Token: 0x040015B4 RID: 5556
	public FsmBool greatherThanBool;

	// Token: 0x040015B5 RID: 5557
	public bool everyFrame;
}
